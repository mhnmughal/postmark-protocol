using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using RehmanMughal.PostmarkProtocol.Gameplay;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.UI
{
    public sealed class SceneUI : MonoBehaviour
    {
        public LevelDefinition[] levels;
        public RoutePresentation route;
        public RectTransform stampGhost;
        public GameObject[] panels;
        public GameObject[] rewardSeals;
        private readonly Dictionary<string, Text> texts = new Dictionary<string, Text>();
        private readonly Dictionary<string, Button> buttons = new Dictionary<string, Button>();
        private string currentPanel;
        private string returnPanel = "MainMenu";
        private int chapter;
        private Command selected = Command.Right;
        private Ink ink;
        private bool erasing;
        private int tutorialStage=-1;
        private static readonly string[] Chapters = { "LOCAL ROUTE", "EXPRESS DESK", "RETURN OFFICE", "INK LAB", "PRIORITY SERVICE", "NIGHT SHIFT" };
        public bool IsPlanning => route != null && !route.IsRunning && currentPanel == "GameHUD";

        private void Awake()
        {
            Time.timeScale = 1;
            Application.targetFrameRate = 60;
            foreach (var label in GetComponentsInChildren<Text>(true)) texts[label.name] = label;
            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                buttons[button.name] = button;
                var name = button.name;
                button.onClick.AddListener(() => Act(name));
            }
            foreach (var slider in GetComponentsInChildren<Slider>(true))
            {
                bool music = slider.name == "MusicVolume";
                slider.SetValueWithoutNotify(music ? SaveService.Current.musicVolume : SaveService.Current.soundVolume);
                slider.onValueChanged.AddListener(value =>
                {
                    if (music) SaveService.Current.musicVolume = value; else SaveService.Current.soundVolume = value;
                    AudioService.ApplySettings();
                    SetText(music ? "MusicValue" : "SoundValue", Mathf.RoundToInt(value * 100) + "%");
                });
            }
            var toggle = GetComponentsInChildren<Toggle>(true).FirstOrDefault();
            if (toggle != null)
            {
                toggle.SetIsOnWithoutNotify(SaveService.Current.haptics);
                toggle.onValueChanged.AddListener(value => { SaveService.Current.haptics = value; SetText("HapticsOnLabel",value ? "ENABLED" : "DISABLED"); SaveService.Save(); if(value) AudioService.Haptic(); });
                SetText("HapticsOnLabel",SaveService.Current.haptics ? "ENABLED" : "DISABLED");
            }
            if (stampGhost != null) stampGhost.gameObject.SetActive(false);
        }
        private void Start()
        {
            AudioService.ApplySettings();
            SetText("SaveNotice",SaveService.LastIssue ?? "");
            if (route != null)
            {
                route.ui = this;
                LoadLevel(PostalApplication.SelectedLevel);
            }
            else
            {
                SetText("ShiftProgress", $"{CompletedCount()} / 36 DELIVERIES FILED");
                Show("MainMenu");
            }
        }
        // Count through the stable service API rather than storing duplicate progress.
        private int CompletedCount() { int count=0; for(int i=1;i<=36;i++) if(SaveService.GetLevel(i).delivered) count++; return count; }
        private void OnApplicationPause(bool paused)
        {
            if (paused) { SaveService.Save(); if(route != null && currentPanel == "GameHUD") Act("Pause"); }
        }
        private void OnDisable() { Time.timeScale = 1; SaveService.Save(); }
        public void SetText(string name, string value) { if (texts.TryGetValue(name, out var label)) label.text = value; }
        public void Show(string panel)
        {
            currentPanel = panel;
            foreach(var item in panels) item.SetActive(item.name == panel);
            if (panel != "GameHUD") { if(stampGhost!=null) stampGhost.gameObject.SetActive(false); }
        }
        private void Act(string action)
        {
            AudioService.Play("ui");
            if(action.StartsWith("Stamp_")) { SelectStamp(action.Substring(6)); return; }
            if(action.StartsWith("Chapter_")) { chapter=int.Parse(action.Substring(8)); RefreshLedger(); Show("LevelLedger"); AudioService.Play("paper"); return; }
            if(action.StartsWith("Level_"))
            {
                int id=chapter*6+int.Parse(action.Substring(6))+1;
                if(id<=SaveService.UnlockedLevel) Begin(id);
                return;
            }
            switch(action)
            {
                case "StartShift": Begin(Mathf.Min(36,SaveService.UnlockedLevel)); break;
                case "Ledger": RefreshChapters(); Show("ChapterPanel"); AudioService.Play("paper"); break;
                case "BackChapters": RefreshChapters(); Show("ChapterPanel"); break;
                case "Settings": returnPanel=currentPanel; SettingsLabels(); Show("SettingsPanel"); break;
                case "Handbook": returnPanel=currentPanel; Show("HandbookPanel"); break;
                case "Credits": Show("CreditsPanel"); break;
                case "BackSettings": SettingsLabels(); Show("SettingsPanel"); break;
                case "Close": SaveService.Save(); Show(returnPanel); break;
                case "Desk": Time.timeScale=1; SceneManager.LoadScene("MainDesk"); break;
                case "BackMain": Show("MainMenu"); break;
                case "ResetProgress": Show("ResetDialog"); break;
                case "CancelReset": Show("SettingsPanel"); break;
                case "ConfirmReset": SaveService.ResetProgress(); if(route!=null) { Time.timeScale=1; SceneManager.LoadScene("MainDesk"); } else { Show("SettingsPanel"); SetText("ShiftProgress","0 / 36 DELIVERIES FILED"); } break;
                case "Dispatch": if(IsPlanning) { Show("GameHUD"); route.Dispatch(); } break;
                case "Pause": if(route!=null) { Time.timeScale=0; Show("PausePanel"); AudioService.Play("pause"); } break;
                case "Resume": Time.timeScale=1; Show("GameHUD"); break;
                case "Restart": Time.timeScale=1; route.Restart(true); Show("GameHUD"); RefreshPlanning(); break;
                case "Review": Time.timeScale=1; route.Restart(false); Show("GameHUD"); RefreshPlanning(); break;
                case "TryAgain": Time.timeScale=1; route.Restart(false); Show("GameHUD"); RefreshPlanning(); break;
                case "Next": Time.timeScale=1; if(route.level.id<36) LoadLevel(route.level.id+1); else SceneManager.LoadScene("MainDesk"); break;
                case "Erase": if(IsPlanning) { erasing=true; SetText("SelectionLabel","LIFT INK • tap a stamped space"); } break;
                case "InkStandard": ink=Ink.Standard; erasing=false; RefreshPlanning(); break;
                case "InkDouble": ink=Ink.Double; erasing=false; RefreshPlanning(); break;
                case "InkPermanent": ink=Ink.Permanent; erasing=false; RefreshPlanning(); break;
            }
        }
        private void Begin(int id) { PostalApplication.SelectedLevel=id; SceneManager.LoadScene("Gameplay"); }
        private void SettingsLabels()
        {
            SetText("MusicValue",Mathf.RoundToInt(SaveService.Current.musicVolume*100)+"%");
            SetText("SoundValue",Mathf.RoundToInt(SaveService.Current.soundVolume*100)+"%");
        }
        private void RefreshChapters()
        {
            for(int i=0;i<6;i++)
            {
                bool unlocked=i*6+1<=SaveService.UnlockedLevel;
                buttons["Chapter_"+i].interactable=unlocked;
                SetText("ChapterLabel_"+i,$"{i+1:00}   {Chapters[i]}\n{(unlocked ? "OPEN LEDGER  →" : "COMPLETE PREVIOUS CHAPTER")}");
            }
        }
        private void RefreshLedger()
        {
            SetText("LedgerTitle",$"CHAPTER {chapter+1:00} / {Chapters[chapter]}");
            for(int i=0;i<6;i++)
            {
                int id=chapter*6+i+1;
                var progress=SaveService.GetLevel(id);
                bool unlocked=id<=SaveService.UnlockedLevel;
                buttons["Level_"+i].interactable=unlocked;
                var definition=levels.FirstOrDefault(l=>l.id==id);
                string seals=progress.delivered ? "DELIVERED" + (progress.efficient ? "  /  EFFICIENT" : "") + (progress.perfect ? "  /  PERFECT" : "") : unlocked ? "AWAITING DISPATCH" : "LOCKED • finish the previous delivery";
                SetText("LevelLabel_"+i,$"{id:00}    {definition?.title ?? "Delivery"}\n{seals}");
            }
        }
        private void LoadLevel(int id)
        {
            var definition=levels.FirstOrDefault(l=>l!=null && l.id==id);
            if(definition==null) { Debug.LogError("Missing level asset: "+id); return; }
            PostalApplication.SelectedLevel=id;
            ink=Ink.Standard; erasing=false;
            selected=definition.availableCommands.Contains(Command.Right) ? Command.Right : definition.availableCommands[0];
            route.Load(definition);
            tutorialStage=definition.id==1 && !SaveService.IsTutorialSeen("level-1") ? 0 : -1;
            SetText("RouteClock","READY TO DISPATCH");
            Show("GameHUD");
            SetText("LevelTitle",$"{id:00} / {definition.title.ToUpperInvariant()}");
            SetText("ChapterName",Chapters[definition.chapter-1]);
            SetText("ParcelLabel",definition.parcel.ToString().ToUpperInvariant()+" MAIL");
            SetText("Lesson",definition.id==1 && !SaveService.IsTutorialSeen("level-1") ? "Pick up the Turn Right stamp." : definition.lesson);
            foreach(Command command in Enum.GetValues(typeof(Command))) buttons["Stamp_"+command].interactable=definition.availableCommands.Contains(command);
            bool lab=definition.chapter>=4;
            buttons["InkDouble"].gameObject.SetActive(lab);
            buttons["InkPermanent"].gameObject.SetActive(lab);
            RefreshPlanning();
        }
        public void SelectStamp(string command)
        {
            if(!IsPlanning) return;
            selected=(Command)Enum.Parse(typeof(Command),command);
            erasing=false;
            if(tutorialStage==0 && selected==Command.Right) tutorialStage=1;
            AudioService.Play("pickup");
            RefreshPlanning();
            if(route.level.id==1 && route.plan.Stamps.Count==0 && !SaveService.IsTutorialSeen("level-1")) SetText("Lesson",selected==Command.Right ? "Stamp the marked route space." : "Choose the Turn Right stamp.");
        }
        public void Place(UnityEngine.Vector2Int position)
        {
            if(!IsPlanning) return;
            string reason;
            bool valid=erasing ? route.plan.TryRemove(position,out reason) : route.plan.TryPlace(position,selected,ink,out reason);
            if(!valid) { SetText("Lesson",reason); AudioService.Play("failure"); AudioService.Haptic(); return; }
            route.RefreshStamps(position);
            if(tutorialStage>=0 && selected==Command.Right && !erasing) tutorialStage=2;
            AudioService.Play(erasing ? "remove" : "stamp"); AudioService.Haptic();
            RefreshPlanning();
            if(route.level.id==1 && route.plan.Stamps.Count>0 && !SaveService.IsTutorialSeen("level-1")) SetText("Lesson","Press Dispatch. Watch your instructions deliver the letter.");
        }
        public void RefreshPlanning()
        {
            if(route==null || route.level==null) return;
            SetText("StampCount",$"{route.plan.Stamps.Count} / {route.level.stampLimit} STAMPS    •    PAR {route.level.par}");
            SetText("SelectionLabel",erasing ? "LIFT INK • tap a stamped space" : $"{selected.ToString().ToUpperInvariant()}  /  {(ink==Ink.Double ? "×2 DOUBLE" : ink==Ink.Permanent ? "LOCK PERMANENT" : "1 STANDARD")}");
            foreach(Command command in Enum.GetValues(typeof(Command)))
            {
                var b=buttons["Stamp_"+command];
                b.interactable=!route.IsRunning && route.level.availableCommands.Contains(command);
                b.GetComponent<Image>().color=command==selected && !erasing ? new Color(.63f,.23f,.16f) : new Color(.17f,.25f,.24f);
            }
            buttons["Dispatch"].interactable=!route.IsRunning;
            buttons["Erase"].interactable=!route.IsRunning;
            foreach(var key in new[]{"InkStandard","InkDouble","InkPermanent"}) buttons[key].interactable=!route.IsRunning;
            foreach(var key in new[]{"Stamp_Right","Dispatch"})
            {
                var outline=buttons[key].GetComponent<Outline>();
                if(outline!=null) outline.enabled=!route.IsRunning && (key=="Stamp_Right" ? tutorialStage==0 : tutorialStage==2);
            }
            route.TutorialSlot(tutorialStage==1);
        }
        public bool CanPlace(Vector2Int position)
        {
            if(!IsPlanning) return false;
            var preview=new PlanState(route.level);
            foreach(var stamp in route.plan.Stamps) preview.TryPlace(stamp.position,stamp.command,stamp.ink,out _);
            return erasing ? preview.TryRemove(position,out _) : preview.TryPlace(position,selected,ink,out _);
        }
        public void DragStamp(Vector2 screen,bool visible)
        {
            if(!IsPlanning || stampGhost==null) return;
            stampGhost.gameObject.SetActive(visible);
            stampGhost.position=screen;
            SetText("GhostLabel",selected.ToString().ToUpperInvariant());
            route.HighlightAt(screen);
        }
        public void DropStamp(Vector2 screen)
        {
            if(stampGhost!=null) stampGhost.gameObject.SetActive(false);
            if(!IsPlanning) return;
            if(route.TryPoint(screen,out var cell)) Place(cell);
            route.ClearHighlight();
        }
        public void DeliveryFinished(SimulationResult result)
        {
            if(result.success)
            {
                SaveService.RecordDelivery(route.level.id,route.plan.Stamps.Count,result.steps,route.level.par,route.level.optimalSteps);
                SaveService.MarkTutorialSeen("level-"+route.level.id);
                SetText("SaveNotice",SaveService.LastIssue ?? "");
                var p=SaveService.GetLevel(route.level.id);
                SetText("ResultTitle",route.level.id==36 ? "SHIFT COMPLETE" : "DELIVERED");
                SetText("ResultReason",route.level.id==36 ? "Thirty-six routes. Every letter found its way home." : "Another letter, safely home.");
                SetText("ResultSeals","[ DELIVERED ]"+(p.efficient ? "   [ EFFICIENT ]" : "")+(p.perfect ? "\n[ PERFECT ROUTE ]" : ""));
                SetText("ResultDetails",$"{route.plan.Stamps.Count} stamps  /  Par {route.level.par}  /  {result.steps} route steps");
                SetText("NextLabel",route.level.id==36 ? "RETURN TO DESK" : "NEXT DELIVERY  →");
                Show("SuccessPanel");
                if(rewardSeals!=null && rewardSeals.Length==3) StartCoroutine(ApplySeals(p.efficient,p.perfect));
            }
            else
            {
                SetText("FailureReason",result.reason);
                Show("FailurePanel");
            }
        }
        private IEnumerator ApplySeals(bool efficient,bool perfect)
        {
            for(int i=0;i<rewardSeals.Length;i++) rewardSeals[i].SetActive(false);
            yield return new WaitForSecondsRealtime(.25f);
            for(int i=0;i<rewardSeals.Length;i++)
            {
                if(i==1 && !efficient || i==2 && !perfect) continue;
                var seal=rewardSeals[i];seal.SetActive(true);AudioService.Play("seal");
                for(float t=0;t<.19f;t+=Time.unscaledDeltaTime)
                {
                    seal.transform.localScale=Vector3.one*Mathf.Lerp(1.65f,1,Mathf.Clamp01(t/.19f));
                    yield return null;
                }
                seal.transform.localScale=Vector3.one;
                yield return new WaitForSecondsRealtime(.12f);
            }
        }
    }
}

