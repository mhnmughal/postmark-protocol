using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using RehmanMughal.PostmarkProtocol.Gameplay;
using RehmanMughal.PostmarkProtocol.UI;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    /// <summary>Opt-in Editor integration exercise. No runner, fixture or storage override ships in a player.</summary>
    [InitializeOnLoad]
    public static class SessionQA
    {
        private sealed class Wait
        {
            public Func<bool> predicate;
            public double deadline;
            public string description;
        }
        private static readonly FieldInfo RepositoryField = typeof(SaveService).GetField("repository", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly List<string> Rows = new List<string>();
        private static readonly List<string> LayoutFailures = new List<string>();
        private static ProfilerRecorder mainThread, gcAllocated, drawCalls;
        private static long mainSum, mainMax, gcSum, gcMax, drawSum, drawMax;
        private static double frameSum, frameMax;
        private static int profileSamples, lastProfileFrame;
        private static bool mainAvailable, gcAvailable, drawAvailable;
        private static IEnumerator routine;
        private static Wait waiting;
        private static SaveRepository originalRepository;
        private static string qaPath;
        private static string error;
        private static double started;
        private static int originalSelectedLevel;
        private static bool originalRunInBackground;
        private static int completed;
        public static string Status { get; private set; } = "Not run.";
        public static bool Running => routine != null;
        private static string ReportPath = "Documentation/SESSION_QA_REPORT.md";
        private static bool layoutReview;
        private static bool sliderReview;
        private static bool storeMode;
        private static readonly List<string> LayoutResults = new List<string>();

        static SessionQA()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () => { if (Running) Finish("INTERRUPTED: script reload", false); };
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode && Running) Finish("INTERRUPTED: Play Mode stopped", false);
            };
        }

        [MenuItem("Postmark Protocol/QA/Run Authored UI Session (Play Mode)")]
        public static void Begin()
        { BeginInternal(false); }
        public static void BeginLayoutReview()
        { BeginInternal(true); }
        public static void BeginSliderReview()
        { BeginInternal(false,true); }
        [MenuItem("Postmark Protocol/QA/Capture App Store Screenshots (Play Mode)")]
        public static void BeginStoreScreenshots()
        { BeginInternal(false,false,true); }
        private static void BeginInternal(bool review,bool sliders=false,bool store=false)
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play Mode before invoking SessionQA.Begin().");
            if (Running) throw new InvalidOperationException("Session QA is already running.");
            _ = SaveService.Current;
            originalRepository = (SaveRepository)RepositoryField.GetValue(null);
            originalSelectedLevel = PostalApplication.SelectedLevel;
            originalRunInBackground = Application.runInBackground;
            qaPath = Path.Combine(Application.temporaryCachePath, "SessionQA-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8), "save.json");
            var isolated = new SaveRepository(qaPath);
            isolated.Load();
            RepositoryField.SetValue(null, isolated);
            Application.runInBackground = true;
            PostalApplication.SelectedLevel = 1;
            Time.timeScale = 1;
            AudioService.ApplySettings();
            Rows.Clear();
            LayoutFailures.Clear();
            LayoutResults.Clear();
            layoutReview=review;
            sliderReview=sliders;
            storeMode=store;
            ReportPath=sliders ? "QA/Slider-Verification.md" : review ? "QA/Policy-UI-Verification.md" : store ? "QA/AppStoreScreenshots/CAPTURE_LOG.md" : "Documentation/SESSION_QA_REPORT.md";
            completed = 0;
            mainSum = mainMax = gcSum = gcMax = drawSum = drawMax = 0;
            frameSum = frameMax = 0;
            profileSamples = 0;
            lastProfileFrame = -1;
            mainThread = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 1);
            gcAllocated = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 1);
            drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1);
            mainAvailable = mainThread.Valid;
            gcAvailable = gcAllocated.Valid;
            drawAvailable = drawCalls.Valid;
            error = null;
            started = EditorApplication.timeSinceStartup;
            Status = "Starting isolated authored-UI session.";
            Application.logMessageReceived += CaptureError;
            routine = sliders ? SliderSweep() : review ? LayoutSweep() : store ? StoreScreenshotSweep() : Run();
            EditorApplication.update += Tick;
            Debug.Log("POSTMARK SESSION QA STARTED: isolated save=" + qaPath);
        }

        public static void Cancel() { if (Running) Finish("CANCELLED by operator", true); }

        private static void CaptureError(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                error = message + "\n" + stack;
        }

        private static void Tick()
        {
            if (!Running) return;
            try
            {
                if (error != null) throw new InvalidOperationException("Unity error during session: " + error);
                ProfileFrame();
                if (waiting != null)
                {
                    if (!waiting.predicate())
                    {
                        if (EditorApplication.timeSinceStartup > waiting.deadline) throw new TimeoutException(waiting.description);
                        return;
                    }
                    waiting = null;
                }
                if (!routine.MoveNext()) { Finish(LayoutFailures.Count == 0 ? "PASS" : "FAIL: " + LayoutFailures.Count + " layout audit findings", true); return; }
                waiting = routine.Current as Wait;
            }
            catch (Exception exception) { Finish("FAIL: " + exception.Message, true); }
        }

        private static Wait Until(Func<bool> predicate, string description, double timeout = 90)
            => new Wait { predicate = predicate, description = description, deadline = EditorApplication.timeSinceStartup + timeout };
        private static Wait Delay(double seconds)
        {
            double until = EditorApplication.timeSinceStartup + seconds;
            return Until(() => EditorApplication.timeSinceStartup >= until, "Editor frame delay", seconds + 10);
        }
        private static SceneUI UI => UnityEngine.Object.FindAnyObjectByType<SceneUI>();
        private static bool Panel(string name) => UI != null && UI.panels.Any(p => p != null && p.name == name && p.activeInHierarchy);
        private static Button Button(string name)
        {
            var match = UI.GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == name && b.gameObject.activeInHierarchy);
            Check(match != null, "Missing visible authored button: " + name);
            return match;
        }
        private static void Click(string name)
        {
            var button = Button(name);
            Check(button.interactable, "Authored button unexpectedly disabled: " + name);
            button.onClick.Invoke();
        }
        private static void Check(bool passed, string description)
        { if (!passed) throw new InvalidOperationException(description); }
        private static void Place(StampPlacement stamp)
        {
            if(UI.route.level.chapter>=4) { Click("ChooseInk"); Click("Ink" + stamp.ink); }
            Click("Stamp_" + stamp.command);
            UI.Place(stamp.position);
            Check(UI.route.plan.Stamps.Any(s => s.position == stamp.position && s.command == stamp.command && s.ink == stamp.ink),
                "Authored controls did not place expected stamp at " + stamp.position);
        }
        private static SaveRepository ReadDisk()
        { var disk = new SaveRepository(qaPath); disk.Load(); return disk; }
        private static void Audit(string context)
        {
            string result = VisualQA.LayoutAudit();
            LayoutResults.Add(context+": "+result);
            if (!result.Contains(": PASS ")) LayoutFailures.Add(context + ": " + result);
        }
        private static IEnumerator SliderSweep()
        {
            foreach(var size in new[]{new Vector2Int(1136,640),new Vector2Int(2778,1284),new Vector2Int(2732,2048)})
            foreach(var scene in new[]{"MainDesk","Gameplay"})
            {
                Status="Slider checks "+scene+" "+size;
                VisualQA.Resolution(size.x,size.y);
                yield return Until(()=>Screen.width==size.x && Screen.height==size.y,"Slider resolution",20);
                SaveService.Current.musicVolume=.9436195f;
                SaveService.Current.soundVolume=.7557976f;
                SceneManager.LoadScene(scene);
                yield return Until(()=>UI!=null && (UI.route==null)==(scene=="MainDesk"),"Slider scene",20);
                yield return Delay(.4);
                UI.Show(scene=="MainDesk" ? "MainMenu" : "PausePanel");Click("Settings");
                yield return Delay(.3);
                AudioAuthoring.ValidatePlayback();
                Check(Math.Abs(SaveService.Current.musicVolume-.94f)<.0001f && Math.Abs(SaveService.Current.soundVolume-.76f)<.0001f,"Legacy fractional settings not aligned to displayed percent");
                foreach(var slider in UI.GetComponentsInChildren<Slider>())
                {
                    var track=(RectTransform)slider.transform.Find("Track");
                    foreach(int percent in new[]{0,1,25,50,76,94,99,100})
                    {
                        // Tap the generous hit area above the visible thumb, then drag to the same X.
                        var position=(Vector2)track.TransformPoint(new Vector3(Mathf.Lerp(track.rect.xMin,track.rect.xMax,percent/100f),32,0));
                        var pointer=new PointerEventData(EventSystem.current){position=position,button=PointerEventData.InputButton.Left};
                        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
                        Check(hits.Count>0 && hits[0].gameObject.GetComponentInParent<Slider>()==slider,"Slider hit area missed");
                        pointer.pointerPressRaycast=hits[0];pointer.pointerCurrentRaycast=hits[0];
                        slider.OnPointerDown(pointer);CheckSlider(slider,percent);
                        pointer.position=(Vector2)track.TransformPoint(new Vector3(Mathf.Lerp(track.rect.xMin,track.rect.xMax,(100-percent)/100f),32,0));
                        slider.OnDrag(pointer);CheckSlider(slider,100-percent);
                        pointer.position=(Vector2)track.TransformPoint(new Vector3(track.rect.xMin-80,32,0));
                        slider.OnDrag(pointer);CheckSlider(slider,0);
                        pointer.position=(Vector2)track.TransformPoint(new Vector3(track.rect.xMax+80,32,0));
                        slider.OnDrag(pointer);CheckSlider(slider,100);
                        slider.OnPointerUp(pointer);
                    }
                }
                UI.GetComponentsInChildren<Slider>().Single(s=>s.name=="MusicVolume").value=94;
                UI.GetComponentsInChildren<Slider>().Single(s=>s.name=="SoundVolume").value=76;
                Click("Close");
                var disk=ReadDisk();
                Check(Math.Abs(disk.Data.musicVolume-.94f)<.0001f && Math.Abs(disk.Data.soundVolume-.76f)<.0001f,"Slider values not persisted");
                RepositoryField.SetValue(null,disk);
                SceneManager.LoadScene(scene);
                yield return Until(()=>UI!=null && (UI.route==null)==(scene=="MainDesk"),"Slider reload",20);
                yield return Delay(.4);UI.Show(scene=="MainDesk" ? "MainMenu" : "PausePanel");Click("Settings");yield return Delay(.3);
                CheckSlider(UI.GetComponentsInChildren<Slider>().Single(s=>s.name=="MusicVolume"),94);
                CheckSlider(UI.GetComponentsInChildren<Slider>().Single(s=>s.name=="SoundVolume"),76);
                Check(Screen.width==size.x && Screen.height==size.y,"Game View changed during slider checks");
                Audit(scene+" sliders");
                if(scene=="MainDesk") { VisualQA.Capture("slider-settings-"+size.x+"x"+size.y);yield return Delay(.4); }
                LayoutResults.Add(scene+" "+size+": tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS");
                completed++;
            }
        }
        private static void CheckSlider(Slider slider,int percent)
        {
            Check(slider.value==percent && Math.Abs(slider.normalizedValue-percent/100f)<.0001f,"Slider percent mismatch");
            var track=(RectTransform)slider.transform.Find("Track");
            float left=track.TransformPoint(new Vector3(track.rect.xMin,0,0)).x;
            float right=track.TransformPoint(new Vector3(track.rect.xMax,0,0)).x;
            float expected=Mathf.Lerp(left,right,percent/100f);
            var corners=new Vector3[4];slider.fillRect.GetWorldCorners(corners);
            Check(Math.Abs(corners[0].x-left)<.1f && Math.Abs(corners[3].x-expected)<.1f,"Fill does not match visible track percent");
            Check(Math.Abs(slider.handleRect.TransformPoint(slider.handleRect.rect.center).x-expected)<.1f,"Thumb does not match visible track percent");
            bool music=slider.name=="MusicVolume";
            Check(UI.GetComponentsInChildren<Text>().Single(t=>t.name==(music ? "MusicValue" : "SoundValue")).text==percent+"%","Slider label mismatch");
            Check(Math.Abs((music ? SaveService.Current.musicVolume : SaveService.Current.soundVolume)-percent/100f)<.0001f,"Saved volume mismatch");
            var mixer=UnityEngine.Object.FindAnyObjectByType<AudioService>().mixer;
            foreach(var key in new[]{"MusicVolume","SFXVolume","UIVolume"})
                Check(mixer.GetFloat(key,out var db) && Math.Abs(db-AudioService.Decibels(key=="MusicVolume" ? SaveService.Current.musicVolume : SaveService.Current.soundVolume))<.01f,"Mixer mismatch: "+key);
        }
        private static IEnumerator LayoutSweep()
        {
            foreach(var size in new[]{new Vector3Int(1136,640,2),new Vector3Int(2778,1284,3),new Vector3Int(2732,2048,2),new Vector3Int(1278,1044,1)})
            {
                Status="Layout sweep "+size.x+"x"+size.y;
                VisualQA.Resolution(size.x,size.y);
                yield return Until(()=>Screen.width==size.x && Screen.height==size.y,"Exact Game View resolution",20);
                SceneManager.LoadScene("MainDesk");
                yield return Until(()=>UI!=null && UI.route==null,"Main desk for layout",20);
                yield return Delay(.4);
                foreach(var panel in UI.panels)
                {
                    UI.Show(panel.name);
                    yield return Delay(.3);
                    Audit("Menu "+panel.name);
                    if(panel.name=="MainMenu" || panel.name=="IntroPanel")
                    { VisualQA.Capture("policy-"+panel.name+"-"+size.x+"x"+size.y);yield return Delay(.4); }
                }
                UI.Show("MainMenu");Click("Settings");
                Check(Panel("SettingsPanel"),"Settings did not open");
                Check(!UI.GetComponentsInChildren<Button>(true).Any(b=>b.name=="Privacy" || b.name=="Support"),"Removed Settings buttons remain");
                Check(!UI.panels.Any(p=>p.name=="PrivacyPanel" || p.name=="SupportPanel"),"Removed information panels remain");
                Click("ResetProgress");
                SaveService.RecordDelivery(1,1,5,1,5);
                Directory.CreateDirectory(qaPath+".tmp");
                Click("ConfirmReset");
                Check(Panel("ResetDialog") && SaveService.GetLevel(1).delivered,"Failed reset lost session data or left the dialog");
                Check(UI.GetComponentsInChildren<Text>().Any(t=>t.name=="ResetStatus" && t.text.Contains("not saved")),"Reset failure message missing");
                yield return Delay(.3);Audit("Reset failure");
                Directory.Delete(qaPath+".tmp");Click("ConfirmReset");
                Check(Panel("SettingsPanel") && !ReadDisk().GetLevel(1).delivered,"Reset retry did not persist");
                yield return Delay(.3);
                VisualQA.Capture("policy-settings-"+size.x+"x"+size.y);
                yield return Delay(.4); // CaptureScreenshot renders on a later frame; do not switch scenes yet.
                SaveService.MarkTutorialSeen("intro");
                PostalApplication.SelectedLevel=1;SceneManager.LoadScene("Gameplay");
                yield return Until(()=>Panel("GameHUD") && UI.route!=null,"Gameplay for layout",20);
                yield return Delay(.4);
                float minPoints=UI.GetComponentsInChildren<Selectable>().Min(b=>((RectTransform)b.transform).rect.height*UI.GetComponent<Canvas>().scaleFactor/size.z);
                Check(minPoints>=44,"Gameplay touch target below 44 simulated logical points: "+minPoints);
                LayoutResults.Add("Control height at "+size.x+"x"+size.y+" / "+size.z+" scale: minimum "+minPoints.ToString("F1")+" logical points (Editor simulation, not physical measurement)");
                foreach(var level in UI.levels)
                {
                    typeof(SceneUI).GetMethod("LoadLevel",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(UI,new object[]{level.id});
                    yield return Delay(.3);
                    Audit("Level "+level.id+" planning");
                    if(level.id==1)
                    {
                        var slot=level.slots[0].position;
                        Vector2 near=UI.route.gameCamera.WorldToScreenPoint(UI.route.World(slot));
                        near.x-=35*UI.GetComponent<Canvas>().scaleFactor;
                        Check(UI.route.TryPoint(near,out var chosen) && chosen==slot,"Expanded route-slot hit region failed");
                    }
                    if(level.id==1 || level.id==36) { VisualQA.Capture("policy-level"+level.id+"-"+size.x+"x"+size.y);yield return Delay(.15); }
                }
                foreach(var panel in UI.panels.Where(p=>p.name!="GameHUD"))
                { UI.Show(panel.name);yield return Delay(.3);Audit("Gameplay "+panel.name); }
                UI.Show("GameHUD");Click("ChooseInk");Click("InkDouble");
                Check(UI.IsPlanning,"Ink selection did not return to route");
                if(size.x==1136)
                {
                    foreach(int id in new[]{1,19,34,36})
                    {
                        typeof(SceneUI).GetMethod("LoadLevel",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(UI,new object[]{id});
                        yield return Delay(.3);
                        foreach(var stamp in UI.route.level.knownSolution) Place(stamp);
                        Click("Dispatch");
                        yield return Until(()=>!UI.route.IsRunning,"Delivery "+id,90);
                        Check(Panel("SuccessPanel") && ReadDisk().GetLevel(id).perfect,"Delivery/seal/save failed: "+id);
                        yield return Delay(1);
                        Audit("Live delivery "+id);
                        VisualQA.Capture("policy-delivered"+id+"-"+size.x+"x"+size.y);
                        LayoutResults.Add("Live authored-control delivery "+id+": PASS delivery, perfect seal and file reload");
                        completed++;
                    }
                }
                WriteReport("RUNNING");
            }
        }
        private static IEnumerator StoreScreenshotSweep()
        {
            SaveService.MarkTutorialSeen("intro");
            var devices = new (string folder,int w,int h)[]
            {
                ("QA/AppStoreScreenshots/iPhone-6.9", 2868, 1320),
                ("QA/AppStoreScreenshots/iPad-13", 2752, 2064),
            };
            foreach (var device in devices)
            {
                Status = "App Store screenshots " + device.folder;
                VisualQA.Resolution(device.w, device.h);
                yield return Until(() => Screen.width == device.w && Screen.height == device.h, "Screenshot resolution", 20);
                SaveService.ResetProgress(); // fresh 0/36 for every device's Main Menu shot, not whatever the previous device left behind
                SceneManager.LoadScene("MainDesk");
                yield return Until(() => UI != null && UI.route == null, "Main desk for screenshots", 20);
                yield return Delay(.4);
                UI.Show("MainMenu");
                yield return Delay(.3);
                VisualQA.Capture("01-MainMenu-" + device.w + "x" + device.h, device.folder);
                yield return Delay(.4);
                completed++;
                SaveService.RecordDelivery(35, 1, 5, 1, 5); // unlock level 36 for its screenshot
                PostalApplication.SelectedLevel = 1;
                SceneManager.LoadScene("Gameplay");
                yield return Until(() => Panel("GameHUD") && UI.route != null, "Gameplay for screenshots", 20);
                yield return Delay(.4);
                foreach (int id in new[] { 1, 36 })
                {
                    typeof(SceneUI).GetMethod("LoadLevel", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(UI, new object[] { id });
                    yield return Delay(.3);
                    VisualQA.Capture("0" + (id == 1 ? 2 : 3) + "-Level" + id + "-" + device.w + "x" + device.h, device.folder);
                    yield return Delay(.4);
                    completed++;
                }
                UI.Show("PausePanel");
                Click("Settings");
                yield return Delay(.3);
                VisualQA.Capture("04-Settings-" + device.w + "x" + device.h, device.folder);
                yield return Delay(.4);
                completed++;
            }
        }
        private static string Capture(int id, string state)
        {
            Directory.CreateDirectory("QA/Playthrough");
            var path = Path.GetFullPath("QA/Playthrough/level-" + id.ToString("00") + "-" + state + ".png");
            if (File.Exists(path)) File.Delete(path); // Exact screenshot file from a previous QA run only.
            ScreenCapture.CaptureScreenshot(path);
            return path;
        }
        private static void ProfileFrame()
        {
            if (Time.frameCount == lastProfileFrame || Time.timeScale <= 0) return;
            var ui = UI;
            if (ui == null || ui.route == null || !ui.route.IsRunning) return;
            lastProfileFrame = Time.frameCount;
            profileSamples++;
            frameSum += Time.unscaledDeltaTime * 1000;
            frameMax = Math.Max(frameMax, Time.unscaledDeltaTime * 1000);
            if (mainAvailable) { long v = mainThread.LastValue; mainSum += v; mainMax = Math.Max(mainMax, v); }
            if (gcAvailable) { long v = gcAllocated.LastValue; gcSum += v; gcMax = Math.Max(gcMax, v); }
            if (drawAvailable) { long v = drawCalls.LastValue; drawSum += v; drawMax = Math.Max(drawMax, v); }
        }

        private static IEnumerator Run()
        {
            // Pin a real landscape resolution before anything else. Without this, Screen.width/height tracks
            // whatever the headless Game View panel happens to be sized at (it drifts across an unattended run
            // and can land on a degenerate size), which made VisualQA.LayoutAudit report false "outside screen"
            // failures unrelated to actual UI layout. See SESSION_QA_REPORT.md history for the reproduction.
            VisualQA.Resolution(1600, 900);
            // This is fixture setup; every navigation assertion below uses the authored UI.
            SceneManager.LoadScene("MainDesk");
            // A fresh isolated save has not seen the first-launch intro, so IntroPanel shows before MainMenu.
            yield return Until(() => Panel("IntroPanel") || Panel("MainMenu"), "MainDesk startup", 20);
            if (Panel("IntroPanel")) { Audit("Intro"); Click("GotIt"); }
            yield return Until(() => Panel("MainMenu"), "MainMenu after intro", 20);
            AudioAuthoring.ValidatePlayback();
            Audit("Main menu");
            Click("Settings");
            Check(Panel("SettingsPanel"), "Settings did not open.");
            Audit("Settings");
            var sliders = UI.GetComponentsInChildren<Slider>(true);
            sliders.Single(s => s.name == "MusicVolume").normalizedValue = 0.21f;
            sliders.Single(s => s.name == "SoundVolume").normalizedValue = 0.37f;
            UI.GetComponentsInChildren<Toggle>(true).Single().isOn = false;
            Click("ResetProgress");
            Check(Panel("ResetDialog"), "Reset confirmation missing.");
            Audit("Reset confirmation");
            Click("CancelReset");
            Click("Close");
            var settings = ReadDisk().Data;
            Check(Math.Abs(settings.musicVolume - 0.21f) < 0.001f && Math.Abs(settings.soundVolume - 0.37f) < 0.001f && !settings.haptics,
                "Authored settings did not persist.");
            Click("Handbook");
            Check(Panel("HandbookPanel"), "Handbook did not open.");
            Audit("Handbook");
            Click("Close");
            Click("Ledger");
            Audit("Chapters");
            Check(Button("Chapter_0").interactable && !Button("Chapter_1").interactable, "Fresh-save chapter locks incorrect.");
            Click("Chapter_0");
            Audit("Ledger");
            Check(Button("Level_0").interactable && !Button("Level_1").interactable, "Fresh-save level locks incorrect.");
            Click("Level_0");
            yield return Until(() => Panel("GameHUD") && UI.route != null && UI.route.level.id == 1, "First delivery load", 20);

            for (int id = 1; id <= 36; id++)
            {
                Status = "Level " + id + "/36: restart and empty-plan return.";
                var route = UI.route;
                Check(route.level.id == id && UI.IsPlanning, "Wrong delivery opened: expected " + id);
                var solution = route.level.knownSolution;
                Check(solution.Length > 0, "Missing authored solution.");
                Place(solution[0]);
                yield return Delay(0.28);
                Click("Pause");
                Check(Panel("PausePanel") && Time.timeScale == 0, "Pause did not freeze planning.");
                Click("Restart");
                Check(route.plan.Stamps.Count == 0 && Panel("GameHUD") && Time.timeScale == 1, "Restart did not clear the sheet.");

                Click("Dispatch");
                Check(route.IsRunning, "Dispatch did not start presentation.");
                yield return Until(() => !route.IsRunning, "Empty-plan return on level " + id);
                Check(Panel("FailurePanel"), "Empty plan did not return on level " + id);
                Audit("Level " + id + " returned");
                Check(!SaveService.GetLevel(id).delivered, "Failed delivery awarded progress on level " + id);
                Click("Review");
                Check(UI.IsPlanning, "Review did not restore planning.");

                Status = "Level " + id + "/36: solution, in-motion pause, seals and persistence.";
                foreach (var stamp in solution)
                {
                    Place(stamp);
                    yield return Delay(0.28);
                }
                Audit("Level " + id + " planning");
                string planningShot = Capture(id, "planning");
                yield return Until(() => File.Exists(planningShot), "Planning screenshot write", 15);
                Click("Dispatch");
                yield return Delay(0.3);
                Check(route.IsRunning, "Route completed before pause test.");
                Click("Pause");
                Audit("Level " + id + " pause");
                var frozen = route.parcel.position;
                yield return Delay(0.2);
                Check(Time.timeScale == 0 && route.IsRunning && Vector3.Distance(frozen, route.parcel.position) < 0.0001f,
                    "Parcel moved while paused on level " + id);
                Click("Resume");
                Check(Time.timeScale == 1 && Panel("GameHUD"), "Resume did not restore gameplay.");
                yield return Until(() => !route.IsRunning, "Normal delivery presentation on level " + id);
                Check(Panel("SuccessPanel"), "Known solution did not show success on level " + id);
                Audit("Level " + id + " delivered");
                string resultShot = Capture(id, "delivered");
                yield return Until(() => File.Exists(resultShot), "Delivery screenshot write", 15);
                var record = SaveService.GetLevel(id);
                Check(record.delivered && record.efficient && record.perfect, "Known solution missing earned seals on level " + id);
                Check(SaveService.UnlockedLevel == Math.Min(36, id + 1), "Wrong level unlock after " + id);
                var disk = ReadDisk();
                Check(disk.GetLevel(id).perfect && disk.Data.unlockedLevel == Math.Min(36, id + 1), "File persistence missing level " + id);
                Check(disk.Data.tutorialSeen.Contains("level-" + id), "Tutorial completion not persisted for level " + id);

                Status = "Level " + id + "/36: replay and menu/ledger round trip.";
                Click("Review");
                Check(route.plan.Stamps.Count == solution.Length && UI.IsPlanning, "Replay lost the stamped plan.");
                Click("Dispatch");
                yield return Until(() => !route.IsRunning, "Replay presentation on level " + id);
                Check(Panel("SuccessPanel") && SaveService.GetLevel(id).perfect, "Replay lost success or seals.");
                Click("Next");
                if (id < 36)
                {
                    Check(Panel("GameHUD") && UI.route.level.id == id + 1, "Next Delivery did not open the next route.");
                    Click("Pause");
                    Click("Desk");
                    yield return Until(() => Panel("MainMenu"), "Return to desk after level " + id, 20);
                    Check(AudioService.Available, "Audio service lost on scene transition.");
                    Click("Ledger");
                    Click("Chapter_" + (id / 6));
                    Click("Level_" + (id % 6));
                    int expected = id + 1;
                    yield return Until(() => Panel("GameHUD") && UI.route != null && UI.route.level.id == expected, "Ledger reopened next delivery", 20);
                }
                else yield return Until(() => Panel("MainMenu"), "Final route returned to desk", 20);
                completed++;
                Rows.Add("| " + id.ToString("00") + " | PASS | PASS | PASS | PASS | PASS | PASS | PASS |");
                Debug.Log("POSTMARK SESSION QA LEVEL " + id.ToString("00") + " PASS (authored UI + live presentation + isolated file save)");
                WriteReport("RUNNING");
            }

            AudioAuthoring.ValidatePlayback();
            Check(ReadDisk().Data.levels.Count(p => p.delivered) == 36, "Not all 36 delivery records survived file reload.");
            Click("Settings");
            Click("ResetProgress");
            Click("ConfirmReset");
            var reset = ReadDisk().Data;
            Check(reset.unlockedLevel == 1 && reset.levels.All(p => !p.delivered) && reset.tutorialSeen.Count == 0, "Confirmed reset did not clear persisted progress.");
            Check(Math.Abs(reset.musicVolume - 0.21f) < 0.001f && Math.Abs(reset.soundVolume - 0.37f) < 0.001f && !reset.haptics,
                "Confirmed reset lost settings.");
            Click("Close");
        }

        private static void Finish(string result, bool returnToDesk)
        {
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= CaptureError;
            (routine as IDisposable)?.Dispose();
            routine = null;
            waiting = null;
            mainThread.Dispose();
            gcAllocated.Dispose();
            drawCalls.Dispose();
            Time.timeScale = 1;
            // Stop live delivery before restoring the user's repository, including cancellation.
            // Otherwise a delayed DeliveryFinished callback could save QA progress to the real file.
            var view = UI;
            if (view != null && view.route != null)
            {
                view.route.StopAllCoroutines();
            }
            if (originalRepository != null) RepositoryField.SetValue(null, originalRepository);
            originalRepository = null;
            PostalApplication.SelectedLevel = originalSelectedLevel;
            Application.runInBackground = originalRunInBackground;
            AudioService.ApplySettings();
            Status = result + "; " + completed + (sliderReview ? "/6 slider scene/resolution checks" : layoutReview ? "/4 representative deliveries" : storeMode ? "/8 App Store screenshots" : "/36 levels") + "; original user save restored; report=" + ReportPath;
            WriteReport(result);
            if (returnToDesk && EditorApplication.isPlaying) SceneManager.LoadScene("MainDesk");
            Debug.Log("POSTMARK SESSION QA " + Status);
        }

        private static void WriteReport(string result)
        {
            if(storeMode)
            {
                var log = new StringBuilder();
                log.AppendLine("# App Store screenshot capture log\n");
                log.AppendLine("UTC: " + DateTime.UtcNow.ToString("u") + "  \nResult: **" + result.Replace("\n", " ") + "**  \nCaptured: " + completed + "/8\n");
                log.AppendLine("Captured from the live authored UI in an isolated Play Mode session (real menu, real level geometry, real settings panel), not composited or edited. Test save: `" + qaPath + "`.\n");
                log.AppendLine("| Folder | Device size | Source |\n| --- | --- | --- |");
                log.AppendLine("| `QA/AppStoreScreenshots/iPhone-6.9/` | 2868x1320 (iPhone 6.9\" display, landscape) | Main Menu, Level 1, Level 36, Settings |");
                log.AppendLine("| `QA/AppStoreScreenshots/iPad-13/` | 2752x2064 (iPad 13\" display, landscape) | Main Menu, Level 1, Level 36, Settings |");
                log.AppendLine("\nGame is landscape-only, so both sizes are captured landscape (width x height swapped from Apple's portrait-listed spec). No text or content was added or edited.");
                if (error != null) log.AppendLine("\nCaptured Unity error:\n```text\n" + error + "\n```");
                Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
                File.WriteAllText(ReportPath, log.ToString());
                return;
            }
            if(sliderReview)
            {
                File.WriteAllText(ReportPath,"# Audio slider verification\n\nUTC: "+DateTime.UtcNow.ToString("u")+"\n\nResult: **"+result+"**; "+completed+"/6 scene/resolution checks.\n\nNative Unity pointer-event checks in Editor Play Mode, not physical touch or subjective listening. Both sliders: 0, 1, 25, 50, 76, 94, 99 and 100 percent; taps, reverse drags, out-of-range clamping, matching track/fill/handle/label/save/mixer values, independent Music/SFX/UI buses, old fractional settings, file reload, real audio-source playback and layout checks. Original user repository restored at completion. No iOS build/export or signing.\n\n"+string.Join("\n",LayoutResults.Select(s=>"- "+s))+"\n");
                return;
            }
            if(layoutReview)
            {
                File.WriteAllText(ReportPath,"# Policy UI verification\n\nUTC: "+DateTime.UtcNow.ToString("u")+"\n\nResult: **"+result+"**\n\nEditor-only checks; no physical testing, iOS build/export, signing or login. All 36 levels receive planning-layout checks at four measured resolutions. Four representative deliveries use live presentation, authored controls, seal checks and isolated file saves. Settings button/panel removal, reset write failure and retry are exercised. User save is restored at completion.\n\nLayout findings: "+LayoutFailures.Count+". Live deliveries completed: "+completed+"/4.\n\n"+string.Join("\n",LayoutResults.Select(s=>"- "+s))+"\n");
                return;
            }
            var text = new StringBuilder();
            text.AppendLine("# Automated Play Mode session integration QA\n");
            text.AppendLine("UTC: " + DateTime.UtcNow.ToString("u") + "  \nResult: **" + result.Replace("\n", " ") + "**  \nCompleted levels: " + completed + "/36  \nElapsed: " + (EditorApplication.timeSinceStartup - started).ToString("F1") + " seconds\n");
            text.AppendLine("This is automated Editor Play Mode integration, **not manual playtesting**, physical touch testing, visual QA, an OS relaunch, or iOS device QA. The runner invokes authored Button listeners, SceneUI placement, live RoutePresentation animation, and the local save repository. No direct core-simulator-only result is accepted.\n");
            text.AppendLine("The user save is isolated by an Editor-only repository swap and restored at termination. Test save: `" + qaPath + "`. Each persisted record is independently read from disk.\n");
            text.AppendLine("| Level | Empty-plan return | Restart/review | Known solution | Pause/resume | Replay | Seals/file save | Next/menu/ledger |\n| --- | --- | --- | --- | --- | --- | --- | --- |");
            foreach (var row in Rows) text.AppendLine(row);
            text.AppendLine("\nSuccessful full completion additionally checks settings controls and file persistence, handbook, initial locks, actual live mixer mute/half-volume response before and after transitions, and confirmed progress reset retaining settings.");
            text.AppendLine("\n## Screenshots and layout\n\nPlanning and delivered screenshots are saved under `QA/Playthrough/level-XX-planning.png` and `level-XX-delivered.png`. Active text extents and screen bounds are audited on menus and every level's planning, pause, returned and delivered screens. Full audit output is in `QA/LayoutAudit.txt`.\n");
            text.AppendLine("Layout findings: " + LayoutFailures.Count + ".");
            foreach (var finding in LayoutFailures) text.AppendLine("- " + finding);
            text.AppendLine("\n## Editor profiling during live delivery\n\nTarget frame rate: " + Application.targetFrameRate + ". Samples: " + profileSamples + ". Real-time simulation at scale 1; paused frames excluded. These measurements include Editor and QA overhead and do **not** establish iPhone/iPad performance.\n");
            if (profileSamples > 0)
            {
                text.AppendLine("| Counter | Mean | Maximum |\n| --- | --- | --- |");
                text.AppendLine("| Frame time | " + (frameSum / profileSamples).ToString("F3") + " ms | " + frameMax.ToString("F3") + " ms |");
                text.AppendLine("| Main Thread | " + (mainAvailable ? (mainSum / (double)profileSamples / 1000000).ToString("F3") + " ms | " + (mainMax / 1000000.0).ToString("F3") + " ms" : "Unavailable | Unavailable") + " |");
                text.AppendLine("| GC Allocated In Frame | " + (gcAvailable ? (gcSum / (double)profileSamples).ToString("F1") + " B | " + gcMax + " B" : "Unavailable | Unavailable") + " |");
                text.AppendLine("| Draw Calls Count | " + (drawAvailable ? (drawSum / (double)profileSamples).ToString("F1") + " | " + drawMax : "Unavailable | Unavailable") + " |");
            }
            if (error != null) text.AppendLine("\nCaptured Unity error:\n```text\n" + error + "\n```");
            Directory.CreateDirectory(Path.GetDirectoryName(ReportPath));
            File.WriteAllText(ReportPath, text.ToString());
        }
    }
}
