using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace RehmanMughal.PostmarkProtocol
{
    [Serializable]
    public sealed class LevelProgress
    {
        public int id;
        public bool delivered;
        public bool efficient;
        public bool perfect;
        public int bestCommandCount = int.MaxValue;
        public int bestSteps = int.MaxValue;
    }

    [Serializable]
    public sealed class SaveData
    {
        public int version = 2;
        public int unlockedLevel = 1;
        public int unlockedChapter = 1;
        public float musicVolume = 0.45f;
        public float soundVolume = 0.8f;
        public bool haptics = true;
        public List<LevelProgress> levels = new List<LevelProgress>();
        public List<string> tutorialSeen = new List<string>();
    }

    /// <summary>Versioned local data with checksum, atomic replacement and last-good backup.</summary>
    public sealed class SaveRepository
    {
        [Serializable] private sealed class Envelope { public string payload; public string checksum; }
        private readonly string path;
        private bool trustedPrimary;
        private bool futureVersion;
        public SaveData Data { get; private set; } = new SaveData();
        public string LastIssue { get; private set; } = "";
        public SaveRepository(string filePath) { path = filePath; }

        public void Load()
        {
            LastIssue = "";
            futureVersion = false;
            trustedPrimary = false;
            Data = new SaveData();
            if (TryRead(path, out var primary)) { Data = primary; trustedPrimary = true; }
            else if (TryRead(path + ".bak", out var backup))
            {
                Data = backup;
                LastIssue = "Recovered the last saved delivery record.";
            }
            Normalize();
        }

        private bool TryRead(string candidate, out SaveData result)
        {
            result = null;
            try
            {
                if (!File.Exists(candidate)) return false;
                if (new FileInfo(candidate).Length > 1024 * 1024) throw new InvalidDataException("Save exceeds limit.");
                var text = File.ReadAllText(candidate, Encoding.UTF8);
                string payload;
                if (text.Contains("\"payload\""))
                {
                    var envelope = JsonUtility.FromJson<Envelope>(text);
                    if (envelope == null || string.IsNullOrEmpty(envelope.payload) || Hash(envelope.payload) != envelope.checksum)
                        throw new InvalidDataException("Save checksum failed.");
                    payload = envelope.payload;
                }
                else payload = text; // Version 1 stored the model directly.
                if (!payload.TrimStart().StartsWith("{") || !payload.Contains("\"version\""))
                    throw new InvalidDataException("Save model missing.");
                result = new SaveData();
                JsonUtility.FromJsonOverwrite(payload, result); // Missing new fields retain defaults.
                if (result.version < 1) throw new InvalidDataException("Invalid version.");
                futureVersion = result.version > 2;
                if (futureVersion) LastIssue = "This delivery record was saved by a newer version. It will not be overwritten.";
                return true;
            }
            catch (Exception error) when (error is IOException || error is InvalidDataException || error is UnauthorizedAccessException || error is ArgumentException || error is System.Security.SecurityException)
            {
                LastIssue = "The delivery record could not be read; progress remains available for this session.";
                return false;
            }
        }

        private void Normalize()
        {
            if (!futureVersion) Data.version = 2;
            Data.musicVolume = FiniteVolume(Data.musicVolume, 0.45f);
            Data.soundVolume = FiniteVolume(Data.soundVolume, 0.8f);
            if (Data.levels == null) Data.levels = new List<LevelProgress>();
            if (Data.tutorialSeen == null) Data.tutorialSeen = new List<string>();
            var seen = new HashSet<int>();
            Data.levels.RemoveAll(p => p == null || p.id < 1 || p.id > 36 || !seen.Add(p.id));
            Data.tutorialSeen.RemoveAll(string.IsNullOrEmpty);
            Data.unlockedLevel = Mathf.Clamp(Data.unlockedLevel, 1, 36);
            foreach (var p in Data.levels)
            {
                if (p.efficient || p.perfect) p.delivered = true;
                p.bestCommandCount = p.bestCommandCount < 0 ? int.MaxValue : p.bestCommandCount;
                p.bestSteps = p.bestSteps < 0 ? int.MaxValue : p.bestSteps;
                if (p.delivered) Data.unlockedLevel = Mathf.Max(Data.unlockedLevel, Mathf.Min(36, p.id + 1));
            }
            Data.unlockedChapter = (Data.unlockedLevel - 1) / 6 + 1;
        }

        private static float FiniteVolume(float value, float fallback) => float.IsNaN(value) || float.IsInfinity(value) ? fallback : Mathf.Clamp01(value);
        private static string Hash(string text)
        {
            using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public bool Save()
        {
            if (futureVersion) return false;
            var temporary = path + ".tmp";
            try
            {
                Normalize();
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var payload = JsonUtility.ToJson(Data);
                var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Envelope { payload = payload, checksum = Hash(payload) }));
                using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
                if (File.Exists(path)) File.Replace(temporary, path, trustedPrimary ? path + ".bak" : null);
                else File.Move(temporary, path);
                trustedPrimary = true;
                LastIssue = "";
                return true;
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is NotSupportedException || error is System.Security.SecurityException)
            {
                LastIssue = "Progress is safe for this session, but storage is unavailable. Try again before closing.";
                return false;
            }
        }

        public LevelProgress GetLevel(int id)
        {
            if (id < 1 || id > 36) throw new ArgumentOutOfRangeException(nameof(id));
            var progress = Data.levels.Find(p => p.id == id);
            if (progress != null) return progress;
            progress = new LevelProgress { id = id };
            Data.levels.Add(progress);
            return progress;
        }

        public void RecordDelivery(int id, int count, int steps, int par, int optimal)
        {
            if (count < 0 || steps < 0 || par < 0 || optimal < 0) throw new ArgumentOutOfRangeException(nameof(count));
            var progress = GetLevel(id);
            progress.delivered = true;
            progress.efficient |= count <= par;
            progress.perfect |= count <= par && steps <= optimal;
            progress.bestCommandCount = Math.Min(progress.bestCommandCount, count);
            progress.bestSteps = Math.Min(progress.bestSteps, steps);
            Data.unlockedLevel = Mathf.Max(Data.unlockedLevel, Math.Min(36, id + 1));
            Data.unlockedChapter = (Data.unlockedLevel - 1) / 6 + 1;
            Save();
        }

        public bool ResetProgress()
        {
            // Deliberate reset retains the settings the player just chose.
            var previous = Data;
            bool previousFutureVersion = futureVersion;
            Data = new SaveData { musicVolume = Data.musicVolume, soundVolume = Data.soundVolume, haptics = Data.haptics };
            futureVersion = false;
            bool written = Save();
            if (!written)
            {
                Data = previous;
                futureVersion = previousFutureVersion;
                LastIssue = "Reset was not saved. Your session progress is unchanged. Free storage and retry.";
                return false;
            }
            // A second atomic checkpoint ensures recovery cannot resurrect erased progress.
            if (Save()) return true;
            LastIssue = "The reset backup could not be saved. Retry before closing to finish clearing the ledger.";
            return false;
        }
    }

    public static class SaveService
    {
        private static SaveRepository repository;
        private static SaveRepository Repository
        {
            get
            {
                if (repository == null)
                {
                    repository = new SaveRepository(Path.Combine(UnityEngine.Application.persistentDataPath, "postmark-save.json"));
                    repository.Load();
                }
                return repository;
            }
        }
        public static SaveData Current => Repository.Data;
        public static int UnlockedLevel => Current.unlockedLevel;
        public static int UnlockedChapter => Current.unlockedChapter;
        public static string LastIssue => Repository.LastIssue;
        public static LevelProgress GetLevel(int id) => Repository.GetLevel(id);
        public static void RecordDelivery(int id, int count, int steps, int par, int optimal) => Repository.RecordDelivery(id, count, steps, par, optimal);
        public static bool Save() => Repository.Save();
        public static bool ResetProgress() => Repository.ResetProgress();
        public static bool IsTutorialSeen(string key) => Current.tutorialSeen.Contains(key);
        public static void MarkTutorialSeen(string key)
        {
            if (string.IsNullOrEmpty(key) || IsTutorialSeen(key)) return;
            Current.tutorialSeen.Add(key);
            Save();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { repository = null; }
    }
}
