using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    /// <summary>Invoked through MCP in Edit Mode. Creates original source audio and authored asset references.</summary>
    public static class AudioAuthoring
    {
        private const string Root = "Assets/Audio/";
        private const int Rate = 44100;
        private static readonly string[] Names = { "pickup", "stamp", "remove", "paper", "dispatch", "slide", "turn", "wait", "reverse", "jump", "mailbox", "success", "failure", "seal", "ui", "pause", "button", "page" };
        private static readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        [MenuItem("Postmark Protocol/Audio/Build Original Audio")]
        public static void BuildAssets()
        {
            Directory.CreateDirectory(Root);
            for (int i = 0; i < Names.Length; i++)
            {
                var duration = Names[i] == "success" ? 1.4 : Names[i] == "failure" ? 0.7 : Names[i] == "page" || Names[i] == "paper" ? 0.45 : 0.24;
                WriteWave(Root + Names[i] + ".wav", Sound(Names[i], duration, i + 20260826));
            }
            WriteWave(Root + "after_hours.wav", Music());
            AssetDatabase.Refresh();
            foreach (var name in Names.Concat(new[] { "after_hours" }))
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(Root + name + ".wav");
                var settings = importer.defaultSampleSettings;
                settings.loadType = name == "after_hours" ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = name == "after_hours" ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.PCM;
                settings.quality = 0.8f;
                importer.defaultSampleSettings = settings;
                importer.forceToMono = true;
                importer.SaveAndReimport();
            }
            BuildMixer();
            AssetDatabase.SaveAssets();
            Debug.Log("POSTMARK AUDIO: 18 original effects, one original loop, Master/Music/SFX/UI mixer authored.");
        }

        private static AudioMixer BuildMixer()
        {
            var existing = AssetDatabase.LoadAssetAtPath<AudioMixer>(Root + "PostalDesk.mixer");
            if (existing != null) return existing;
            // Unity exposes mixer creation only through its own Editor controller API.
            // Reflect the installed Editor API here; no reflection ships in the player.
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetType("UnityEditor.Audio.AudioMixerController") != null);
            var controller = assembly.GetType("UnityEditor.Audio.AudioMixerController");
            var mixer = (AudioMixer)controller.GetMethod("CreateMixerControllerAtPath", Flags).Invoke(null, new object[] { Root + "PostalDesk.mixer" });
            var master = controller.GetProperty("masterGroup", Flags).GetValue(mixer);
            var groups = new object[4];
            groups[0] = master;
            var labels = new[] { "Master", "Music", "SFX", "UI" };
            for (int i = 1; i < groups.Length; i++)
            {
                groups[i] = controller.GetMethod("CreateNewGroup", Flags).Invoke(mixer, new object[] { labels[i], true });
                var add = controller.GetMethod("AddChildToParent", Flags);
                add.Invoke(add.IsStatic ? null : mixer, new[] { groups[i], master });
            }
            var exposed = assembly.GetType("UnityEditor.Audio.ExposedAudioParameter");
            var array = Array.CreateInstance(exposed, 4);
            for (int i = 0; i < groups.Length; i++)
            {
                var parameter = Activator.CreateInstance(exposed);
                exposed.GetField("guid", Flags).SetValue(parameter, groups[i].GetType().GetMethod("GetGUIDForVolume", Flags).Invoke(groups[i], null));
                exposed.GetField("name", Flags).SetValue(parameter, labels[i] + "Volume");
                array.SetValue(parameter, i);
            }
            controller.GetProperty("exposedParameters", Flags).SetValue(mixer, array);
            EditorUtility.SetDirty(mixer);
            AssetDatabase.SaveAssets();
            return mixer;
        }

        public static void Configure(AudioService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            service.mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(Root + "PostalDesk.mixer");
            if (service.mixer == null) throw new InvalidOperationException("BuildAssets must run before Configure.");
            service.musicSource = Source(service, "Music", true);
            service.musicSource.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "after_hours.wav");
            service.sfxSource = Source(service, "SFX", false);
            service.uiSource = Source(service, "UI", false);
            service.sounds = Names.Select(n => new PostalSound
            {
                eventName = n, clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + n + ".wav"),
                ui = n == "ui" || n == "pause" || n == "button" || n == "page", gain = n == "slide" ? 0.35f : 0.65f
            }).ToArray();
            EditorUtility.SetDirty(service);
        }

        private static AudioSource Source(AudioService service, string group, bool loop)
        {
            var child = service.transform.Find(group);
            if (child == null)
            {
                var go = new GameObject(group);
                go.transform.SetParent(service.transform, false);
                child = go.transform;
            }
            var source = child.GetComponent<AudioSource>();
            if (source == null) source = child.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0;
            source.volume = 1;
            source.outputAudioMixerGroup = service.mixer.FindMatchingGroups(group).Single(g => g.name == group);
            EditorUtility.SetDirty(source);
            return source;
        }

        private static float[] Sound(string name, double duration, int seed)
        {
            var data = new float[(int)(Rate * duration)];
            var random = new System.Random(seed);
            double low = 0;
            for (int i = 0; i < data.Length; i++)
            {
                double t = i / (double)Rate;
                double noise = random.NextDouble() * 2 - 1;
                low = low * 0.86 + noise * 0.14;
                double attack = Math.Min(1, t * 1000);
                double end = Math.Min(1, (duration - t) * 150);
                double value;
                switch (name)
                {
                    case "stamp": case "seal":
                        value = 0.62 * Math.Sin(2 * Math.PI * 94 * t) * Math.Exp(-t * 37) + 0.46 * low * Math.Exp(-t * 65); break;
                    case "paper": case "page": case "slide": case "remove":
                        value = low * 0.65 * Math.Sin(Math.PI * t / duration) * (0.7 + 0.3 * Math.Sin(2 * Math.PI * 23 * t)); break;
                    case "pickup":
                        value = 0.38 * Math.Sin(2 * Math.PI * 310 * t) * Math.Exp(-t * 50) + low * 0.17 * Math.Exp(-t * 25); break;
                    case "dispatch": case "mailbox":
                        value = (Math.Sin(2 * Math.PI * 170 * t) * 0.40 + Math.Sin(2 * Math.PI * 510 * t) * 0.12) * Math.Exp(-t * 21) + low * 0.20 * Math.Exp(-t * 60); break;
                    case "success":
                        value = Chime(t, 523.25, 0) + Chime(t, 659.25, 0.18) + Chime(t, 783.99, 0.36); break;
                    case "failure":
                        value = Chime(t, 261.63, 0) + Chime(t, 196, 0.15); break;
                    case "jump":
                        value = 0.26 * Math.Sin(2 * Math.PI * (230 * t + 650 * t * t)) * Math.Sin(Math.PI * t / duration); break;
                    case "reverse":
                        value = 0.25 * Math.Sin(2 * Math.PI * (500 * t - 550 * t * t)) * Math.Sin(Math.PI * t / duration); break;
                    case "wait":
                        value = 0.2 * Math.Sin(2 * Math.PI * 420 * t) * Math.Exp(-t * 38); break;
                    default:
                        value = 0.28 * Math.Sin(2 * Math.PI * 740 * t) * Math.Exp(-t * 52) + low * 0.10 * Math.Exp(-t * 45); break;
                }
                data[i] = (float)(value * attack * end * 0.60);
            }
            return data;
        }

        private static double Chime(double time, double frequency, double start)
        {
            double t = time - start;
            return t < 0 ? 0 : 0.24 * Math.Sin(2 * Math.PI * frequency * t) * Math.Min(1, t * 150) * Math.Exp(-t * 5);
        }

        private static float[] Music()
        {
            // Original 32-beat motif in C6 / A minor / Fmaj7 / Gsus2, 80 BPM.
            const double duration = 24;
            var data = new float[(int)(Rate * duration)];
            var notes = new[] { 60, 64, 67, 69, 64, 67, 72, 67, 57, 60, 64, 67, 60, 64, 69, 64, 53, 57, 60, 64, 57, 60, 65, 60, 55, 62, 67, 69, 62, 67, 71, 67 };
            for (int i = 0; i < data.Length; i++)
            {
                double t = i / (double)Rate;
                double value = 0;
                int beat = (int)(t / 0.75);
                for (int previous = 0; previous < 4; previous++)
                {
                    int index = (beat - previous + notes.Length) % notes.Length;
                    double age = t - (beat - previous) * 0.75;
                    double frequency = 440 * Math.Pow(2, (notes[index] - 69) / 12.0);
                    value += 0.075 * Math.Sin(2 * Math.PI * frequency * age) * Math.Min(1, age * 60) * Math.Exp(-age * 2.8);
                    value += 0.018 * Math.Sin(2 * Math.PI * frequency * 2 * age) * Math.Exp(-age * 5);
                }
                double edge = Math.Min(1, Math.Min(t, duration - t) / 0.02);
                data[i] = (float)(value * edge);
            }
            return data;
        }

        private static void WriteWave(string path, float[] samples)
        {
            using (var writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + samples.Length * 2);
                writer.Write(Encoding.ASCII.GetBytes("WAVEfmt ")); writer.Write(16); writer.Write((short)1); writer.Write((short)1);
                writer.Write(Rate); writer.Write(Rate * 2); writer.Write((short)2); writer.Write((short)16);
                writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(samples.Length * 2);
                foreach (var sample in samples) writer.Write((short)(Mathf.Clamp(sample, -1, 1) * 32767));
            }
        }

        [MenuItem("Postmark Protocol/Audio/Validate Assets")]
        public static void Validate()
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(Root + "PostalDesk.mixer");
            if (mixer == null) throw new InvalidOperationException("Missing mixer.");
            foreach (var group in new[] { "Master", "Music", "SFX", "UI" })
            {
                if (!mixer.FindMatchingGroups(group).Any(g => g.name == group)) throw new InvalidOperationException("Missing group " + group);
                if (!mixer.GetFloat(group + "Volume", out _)) throw new InvalidOperationException("Missing exposed volume " + group);
            }
            foreach (var name in Names.Concat(new[] { "after_hours" }))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Root + name + ".wav");
                if (clip == null || clip.length <= 0) throw new InvalidOperationException("Missing clip " + name);
                var bytes = File.ReadAllBytes(Root + name + ".wav");
                int peak = 0;
                for (int i = 44; i < bytes.Length - 1; i += 2) peak = Math.Max(peak, Math.Abs((int)BitConverter.ToInt16(bytes, i)));
                if (peak == 0 || peak > 25000) throw new InvalidOperationException("Unsafe/empty peak " + name + ": " + peak);
            }
            Debug.Log("POSTMARK AUDIO QA PASS: all 19 clips imported, nonempty and unclipped; four mixer buses expose real volume parameters.");
        }

        public static void ValidatePlayback()
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Playback check requires Play Mode.");
            var service = UnityEngine.Object.FindAnyObjectByType<AudioService>();
            if (service == null || !AudioService.Available) throw new InvalidOperationException("Persistent audio service missing.");
            if (service.musicSource == null || !service.musicSource.isPlaying || !service.musicSource.loop)
                throw new InvalidOperationException("Ambient loop is not playing.");
            foreach (var source in new[] { service.musicSource, service.sfxSource, service.uiSource })
                if (source == null || source.outputAudioMixerGroup == null) throw new InvalidOperationException("Unrouted audio source.");
            if (service.sounds.Length != Names.Length || service.sounds.Any(s => s.clip == null))
                throw new InvalidOperationException("Missing sound references.");
            float music = SaveService.Current.musicVolume, sound = SaveService.Current.soundVolume;
            try
            {
                SaveService.Current.musicVolume = 0;
                SaveService.Current.soundVolume = 0;
                AudioService.ApplySettings();
                foreach (var key in new[] { "MusicVolume", "SFXVolume", "UIVolume" })
                    if (!service.mixer.GetFloat(key, out var value) || Math.Abs(value + 80) > 0.01f)
                        throw new InvalidOperationException("Mute did not reach the mixer: " + key);
                SaveService.Current.musicVolume = 0.5f;
                SaveService.Current.soundVolume = 0.5f;
                AudioService.ApplySettings();
                foreach (var key in new[] { "MusicVolume", "SFXVolume", "UIVolume" })
                    if (!service.mixer.GetFloat(key, out var value) || Math.Abs(value + 6.0206f) > 0.01f)
                        throw new InvalidOperationException("Volume did not reach the mixer: " + key);
                AudioService.Play("ui");
                if (!service.uiSource.isPlaying) throw new InvalidOperationException("UI sound did not start.");
                AudioService.Play("paper");
                if (!service.sfxSource.isPlaying) throw new InvalidOperationException("SFX sound did not start.");
            }
            finally
            {
                SaveService.Current.musicVolume = music;
                SaveService.Current.soundVolume = sound;
                AudioService.ApplySettings();
            }
            Debug.Log("POSTMARK AUDIO PLAYBACK PASS: live loop, SFX/UI one-shots, authored routing and real mute/half-volume responses verified. Settings restored.");
        }
    }
}
