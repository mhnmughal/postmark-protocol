using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using RehmanMughal.PostmarkProtocol.UI;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    // Edit-mode authoring recipe, invoked through AnkleBreaker MCP. Nothing here enters a player build.
    public static class SceneAuthoring
    {
        static readonly Color Cream=new Color(.94f,.89f,.76f), Dark=new Color(.12f,.22f,.21f), Red=new Color(.64f,.23f,.16f), Muted=new Color(.39f,.43f,.36f);
        static Font font;
        static Material wood,paper,ink,red,metal,rubber,green,night;
        static readonly List<GameObject> panels=new List<GameObject>();
        static Transform safe;
        static SceneUI ui;
        static string Root="Assets/";

        [MenuItem("Postmark/Author production scenes")]
        public static string BuildAll()
        {
            if(EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before authoring.");
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            Materials(); Prefabs(); Bootstrap(); MainDesk(); GameplayScene();
            EditorBuildSettings.scenes=new[]{"Bootstrap","MainDesk","Gameplay"}.Select(n=>new EditorBuildSettingsScene("Assets/Scenes/"+n+".unity",true)).ToArray();
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Scenes/MainDesk.unity");
            return "Authored Bootstrap, MainDesk, Gameplay; saved production UI, environment and reusable gameplay prefabs.";
        }
        static GameObject Empty(string name,Transform parent=null)
        {
            var go=new GameObject(name); if(parent!=null) go.transform.SetParent(parent,false); return go;
        }
        static Material Mat(string name,Color color,float gloss=0)
        {
            string path=Root+"Materials/"+name+".mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(mat==null) { mat=new Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(mat,path); }
            mat.color=color;mat.SetFloat("_Glossiness",gloss); return mat;
        }
        static void Materials()
        {
            wood=Mat("Warm walnut",new Color(.34f,.20f,.12f),.2f); paper=Mat("Ivory cotton paper",Cream,.02f);
            ink=Mat("Postal teal",Dark,.15f); red=Mat("Vermilion ink",Red,.1f);metal=Mat("Brushed brass",new Color(.66f,.49f,.24f),.65f);
            rubber=Mat("Stamp rubber",new Color(.075f,.085f,.078f));green=Mat("Sage desk mat",new Color(.28f,.37f,.29f),.05f);
            night=Mat("Night walnut",new Color(.14f,.20f,.22f),.2f);
            string path="Assets/Art/WalnutGrain.asset";
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if(texture==null)
            {
                texture=new Texture2D(256,256,TextureFormat.RGB24,true);texture.name="Original procedural walnut grain";
                var pixels=new Color[256*256];
                for(int y=0;y<256;y++) for(int x=0;x<256;x++)
                {
                    float grain=Mathf.Sin(y*.62f+Mathf.Sin(x*.031f)*2.2f)*.055f+Mathf.PerlinNoise(x*.04f,y*.33f)*.12f;
                    pixels[y*256+x]=new Color(.75f+grain,.65f+grain,.51f+grain);
                }
                texture.SetPixels(pixels);texture.Apply();texture.wrapMode=TextureWrapMode.Repeat;AssetDatabase.CreateAsset(texture,path);
            }
            wood.mainTexture=texture;wood.mainTextureScale=new Vector2(3,4);night.mainTexture=texture;
        }
        static GameObject Shape(string name,Transform parent,Vector3 pos,Vector3 scale,Material material,PrimitiveType type=PrimitiveType.Cube)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=pos;go.transform.localScale=scale;
            go.GetComponent<Renderer>().sharedMaterial=material;
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }
        static TextMesh WorldText(string name,Transform parent,string content,Vector3 pos,float size,Color color)
        {
            var go=Empty(name,parent);go.transform.localPosition=pos;go.transform.localRotation=Quaternion.Euler(90,0,0);
            var label=go.AddComponent<TextMesh>();label.text=content;label.font=font;label.fontSize=64;label.characterSize=size;label.anchor=TextAnchor.MiddleCenter;label.alignment=TextAlignment.Center;label.color=color;
            go.GetComponent<MeshRenderer>().sharedMaterial=font.material;return label;
        }
        static Transform Stamp(string name,Transform parent,Vector3 pos)
        {
            var root=Empty(name,parent).transform;root.localPosition=pos;
            Shape("Rubber foot",root,new Vector3(0,.08f,0),new Vector3(.75f,.12f,.65f),rubber);
            Shape("Brass plate",root,new Vector3(0,.17f,0),new Vector3(.78f,.1f,.68f),metal);
            Shape("Walnut stem",root,new Vector3(0,.48f,0),new Vector3(.28f,.29f,.28f),wood,PrimitiveType.Cylinder);
            Shape("Polished grip",root,new Vector3(0,.77f,0),new Vector3(.64f,.31f,.51f),wood,PrimitiveType.Sphere);
            return root;
        }
        static Transform Envelope(string name,Transform parent,Vector3 pos)
        {
            var root=Empty(name,parent).transform;root.localPosition=pos;
            Shape("Cotton envelope",root,Vector3.zero,new Vector3(.85f,.09f,.58f),paper);
            var flap=Shape("Fold",root,new Vector3(0,.055f,.08f),new Vector3(.57f,.018f,.015f),metal);flap.transform.localRotation=Quaternion.Euler(0,24,0);
            var flap2=Shape("Fold return",root,new Vector3(.01f,.055f,.08f),new Vector3(.57f,.018f,.015f),metal);flap2.transform.localRotation=Quaternion.Euler(0,-24,0);
            Shape("Postage",root,new Vector3(.27f,.065f,.15f),new Vector3(.17f,.018f,.18f),red);
            WorldText("Parcel mark",root,"→",new Vector3(0,.072f,-.12f),.095f,Dark);
            return root;
        }
        static void Prefabs()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=Empty("RouteCell");
            var cell=root.AddComponent<RouteCell>();
            cell.surface=Shape("Paper tile",root.transform,Vector3.zero,new Vector3(.88f,.035f,.88f),paper).GetComponent<MeshRenderer>();
            var border=Empty("Programmable border",root.transform);cell.slotBorder=border;
            foreach(float side in new[]{-.42f,.42f})
            {
                Shape("Ink border horizontal",border.transform,new Vector3(0,.026f,side),new Vector3(.83f,.012f,.025f),ink);
                Shape("Ink border vertical",border.transform,new Vector3(side,.026f,0),new Vector3(.025f,.012f,.83f),ink);
            }
            cell.label=WorldText("Imprint",root.transform,"+",new Vector3(0,.035f,0),.07f,Dark);
            var collider=root.AddComponent<BoxCollider>();collider.center=new Vector3(0,.03f,0);collider.size=new Vector3(.94f,.12f,.94f);
            PrefabUtility.SaveAsPrefabAsset(root,"Assets/Prefabs/Gameplay/RouteCell.prefab");
            UnityEngine.Object.DestroyImmediate(root);
            var link=Shape("RouteLink",null,Vector3.zero,Vector3.one,ink);PrefabUtility.SaveAsPrefabAsset(link,"Assets/Prefabs/Gameplay/RouteLink.prefab");UnityEngine.Object.DestroyImmediate(link);
            var envelope=Envelope("Letter",null,Vector3.zero);PrefabUtility.SaveAsPrefabAsset(envelope.gameObject,"Assets/Prefabs/Gameplay/Letter.prefab");UnityEngine.Object.DestroyImmediate(envelope.gameObject);
            var stamp=Stamp("RubberStamp",null,Vector3.zero);PrefabUtility.SaveAsPrefabAsset(stamp.gameObject,"Assets/Prefabs/Gameplay/RubberStamp.prefab");
        }
        static void Bootstrap()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            Empty("Bootstrap").AddComponent<PostalApplication>();
            var audio=Empty("Audio").AddComponent<AudioService>();
            AudioAuthoring.Configure(audio);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Bootstrap.unity");
        }
        static Transform Environment(string name,out Camera camera,out Light light,out Renderer desk)
        {
            var root=Empty(name).transform;
            Empty("Systems",root);
            var env=Empty("Environment",root).transform;
            desk=Shape("DeskSurface",env,new Vector3(0,-.35f,0),new Vector3(30,.6f,22),wood).GetComponent<Renderer>();
            Shape("DeskMat",env,new Vector3(-1.3f,.005f,.1f),new Vector3(11.3f,.06f,8.7f),green);
            Shape("RoutePaper",env,new Vector3(-1.35f,.06f,.3f),new Vector3(9.6f,.035f,7.35f),paper);
            // Authored tabletop props around the clear playable paper.
            var rack=Empty("StampRack",env).transform;
            Shape("Rack walnut base",rack,new Vector3(5.2f,.17f,.4f),new Vector3(1.3f,.24f,4.8f),wood);
            for(int i=0;i<4;i++) Stamp("Stamp holder "+(i+1),rack,new Vector3(5.2f,.3f,-1.2f+i*1.05f));
            var pad=Empty("InkPads",env).transform;
            Shape("Ink tin",pad,new Vector3(4.7f,.14f,-3.2f),new Vector3(1.8f,.24f,1.05f),metal);
            Shape("Red felt",pad,new Vector3(4.7f,.28f,-3.2f),new Vector3(1.6f,.08f,.87f),red);
            var props=Empty("Props",env).transform;
            for(int i=0;i<4;i++) { var mail=Envelope("DecorativeMail "+i,props,new Vector3(-7.2f,.14f+i*.1f,-2.8f));mail.localScale=Vector3.one*1.5f;mail.localRotation=Quaternion.Euler(0,i*7-10,0); }
            var pencil=Shape("Pencil",props,new Vector3(-6.8f,.16f,.5f),new Vector3(.14f,1.45f,.14f),metal,PrimitiveType.Cylinder);pencil.transform.localRotation=Quaternion.Euler(85,0,15);
            for(int i=0;i<3;i++)
            {
                var clip=Empty("PaperClip "+i,props).transform;clip.localPosition=new Vector3(-5.2f+i*.4f,.14f,-3.95f);clip.localRotation=Quaternion.Euler(0,i*18,0);
                Shape("Left wire",clip,new Vector3(-.09f,0,0),new Vector3(.03f,.035f,.48f),metal);
                Shape("Right wire",clip,new Vector3(.09f,0,0),new Vector3(.03f,.035f,.48f),metal);
                Shape("Top wire",clip,new Vector3(0,0,.24f),new Vector3(.18f,.035f,.03f),metal);
                Shape("Inner bend",clip,new Vector3(0,0,-.24f),new Vector3(.18f,.035f,.03f),metal);
            }
            var lamp=Empty("DeskLamp",env).transform;
            Shape("Weighted base",lamp,new Vector3(6.6f,.13f,3.6f),new Vector3(1.4f,.18f,1.4f),metal,PrimitiveType.Cylinder);
            Shape("Lamp stem",lamp,new Vector3(6.6f,1.3f,3.6f),new Vector3(.12f,1.25f,.12f),metal,PrimitiveType.Cylinder);
            Shape("Green shade",lamp,new Vector3(6.1f,2.4f,3.6f),new Vector3(1.5f,.55f,.85f),green,PrimitiveType.Sphere);
            var lighting=Empty("Lighting",root).transform;
            light=Empty("Warm desk light",lighting).AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.05f;light.color=new Color(1,.87f,.68f);light.shadows=LightShadows.Soft;light.shadowStrength=.5f;light.transform.rotation=Quaternion.Euler(58,-35,0);
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.57f,.56f,.49f);
            camera=Empty("Main Camera",Empty("CameraRig",root).transform).AddComponent<Camera>();camera.tag="MainCamera";camera.transform.position=new Vector3(0,13,-8);camera.transform.LookAt(new Vector3(0,0,.1f));camera.orthographic=true;camera.orthographicSize=5.45f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.16f,.19f,.16f);camera.nearClipPlane=.1f;camera.farClipPlane=50;camera.allowHDR=false;camera.gameObject.AddComponent<AudioListener>();camera.gameObject.AddComponent<PhysicsRaycaster>();
            return root;
        }
        static RectTransform Rect(string name,Transform parent,Vector2 position,Vector2 size)
        {
            var go=new GameObject(name,typeof(RectTransform));var rect=(RectTransform)go.transform;rect.SetParent(parent,false);rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.anchoredPosition=position;rect.sizeDelta=size;return rect;
        }
        static void Full(RectTransform rect,float padding=0)
        {
            rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=Vector2.one*padding;rect.offsetMax=Vector2.one*-padding;
        }
        static Image Image(string name,Transform parent,Vector2 pos,Vector2 size,Color color)
        {
            var r=Rect(name,parent,pos,size);var im=r.gameObject.AddComponent<Image>();im.color=color;im.raycastTarget=false;return im;
        }
        static Text Label(string name,Transform parent,string value,Vector2 pos,Vector2 size,int sizeFont=28,Color? color=null,TextAnchor alignment=TextAnchor.MiddleLeft)
        {
            var r=Rect(name,parent,pos,size);var text=r.gameObject.AddComponent<Text>();text.font=font;text.text=value;text.fontSize=sizeFont;text.color=color??Dark;text.alignment=alignment;text.raycastTarget=false;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;return text;
        }
        static Button Button(string name,Transform parent,string caption,Vector2 pos,Vector2 size,Color? color=null,string labelName=null,int fontSize=25)
        {
            var image=Image(name,parent,pos,size,color??Dark);image.raycastTarget=true;
            var shadow=image.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(0,0,0,.19f);shadow.effectDistance=new Vector2(3,-5);
            var button=image.gameObject.AddComponent<Button>();var colors=button.colors;colors.highlightedColor=new Color(1.12f,1.1f,1.05f);colors.pressedColor=new Color(.78f,.75f,.67f);colors.disabledColor=new Color(.56f,.56f,.5f,.6f);button.colors=colors;button.targetGraphic=image;button.navigation=new Navigation{mode=Navigation.Mode.None};
            image.gameObject.AddComponent<PaperMotion>();
            var text=Label(labelName??name+"Label",image.transform,caption,Vector2.zero,size-Vector2.one*24,fontSize,color==Cream ? Dark : Cream,TextAnchor.MiddleCenter);
            Full((RectTransform)text.transform,12);
            return button;
        }
        static GameObject Panel(string name, bool paperPanel=true)
        {
            var rect=Rect(name,safe,Vector2.zero,Vector2.zero);Full(rect);panels.Add(rect.gameObject);
            if(paperPanel) rect.gameObject.AddComponent<PanelMotion>();
            if(paperPanel)
            {
                var scrim=Image("Desk dimmer",rect,Vector2.zero,Vector2.zero,new Color(.07f,.12f,.1f,.68f));Full((RectTransform)scrim.transform);scrim.raycastTarget=true;
                var sheet=Image("Cotton paper panel",rect,Vector2.zero,new Vector2(1060,738),Cream);
                var shadow=sheet.gameObject.AddComponent<Shadow>();shadow.effectDistance=new Vector2(8,-12);shadow.effectColor=new Color(0,0,0,.3f);
                Image("Letterhead rule",rect,new Vector2(0,247),new Vector2(964,3),Dark);
                Label("Panel running header",rect,"POSTMARK PROTOCOL     /     POSTAL SERVICE",new Vector2(0,318),new Vector2(964,34),20,Muted);
            }
            return rect.gameObject;
        }
        static void UI(Transform root)
        {
            panels.Clear();
            var canvas=Empty("UI",root).AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,900);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;scaler.matchWidthOrHeight=1;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            ui=canvas.gameObject.AddComponent<SceneUI>();
            safe=Rect("SafeArea",canvas.transform,Vector2.zero,Vector2.zero);Full((RectTransform)safe);safe.gameObject.AddComponent<SafeArea>();
            var audio=Empty("Audio").AddComponent<AudioService>();AudioAuthoring.Configure(audio);
            var eventSystem=Empty("EventSystem",root);eventSystem.AddComponent<EventSystem>();eventSystem.AddComponent<InputSystemUIInputModule>();
            ui.levels=AssetDatabase.FindAssets("t:LevelDefinition").Select(g=>AssetDatabase.LoadAssetAtPath<LevelDefinition>(AssetDatabase.GUIDToAssetPath(g))).OrderBy(l=>l.id).ToArray();
        }
        static void CommonPanels()
        {
            var settings=Panel("SettingsPanel").transform;
            Label("SettingsTitle",settings,"DESK SETTINGS",new Vector2(0,278),new Vector2(960,55),36);
            Slider(settings,"MusicVolume","Music",135,SaveService.Current.musicVolume);
            Slider(settings,"SoundVolume","Sound effects",35,SaveService.Current.soundVolume);
            var toggleRect=Rect("Haptics",settings,new Vector2(328,-65),new Vector2(230,64));
            var toggle=toggleRect.gameObject.AddComponent<Toggle>();
            var bg=Image("Toggle track",toggleRect,Vector2.zero,new Vector2(230,64),Dark);bg.raycastTarget=true;toggle.targetGraphic=bg;
            var check=Image("Haptics check",toggleRect,new Vector2(-75,0),new Vector2(38,38),Red);toggle.graphic=check;
            Label("HapticsOnLabel",toggleRect,"ENABLED",new Vector2(23,0),new Vector2(140,52),22,Cream,TextAnchor.MiddleCenter);
            Label("HapticsLabel",settings,"Haptic feedback",new Vector2(-244,-65),new Vector2(390,64),28);
            Button("ResetProgress",settings,"RESET PROGRESS",new Vector2(-242,-182),new Vector2(438,66),Red);
            Button("Credits",settings,"CREDITS & ORIGINS",new Vector2(242,-182),new Vector2(438,66));
            Label("LocalOnly",settings,"Your deliveries stay on this device. No accounts. No connection required.",new Vector2(0,-251),new Vector2(928,54),22,Muted,TextAnchor.MiddleCenter);
            Button("Close",settings,"CLOSE SETTINGS",new Vector2(0,-313),new Vector2(410,62));
            var reset=Panel("ResetDialog").transform;
            Label("ResetTitle",reset,"CLEAR THE LEDGER?",new Vector2(0,278),new Vector2(960,55),36);
            Label("ResetExplanation",reset,"This removes all delivery progress and postal seals from this device.\n\nMusic, sound and haptic preferences will be kept.\nThis cannot be undone.",new Vector2(0,70),new Vector2(890,250),30,Dark,TextAnchor.MiddleCenter);
            Button("CancelReset",reset,"KEEP MY PROGRESS",new Vector2(-240,-200),new Vector2(435,76));
            Button("ConfirmReset",reset,"CLEAR PROGRESS",new Vector2(240,-200),new Vector2(435,76),Red);
            var handbook=Panel("HandbookPanel").transform;
            Label("HandbookTitle",handbook,"THE ROUTE HANDBOOK",new Vector2(0,278),new Vector2(960,55),36);
            Label("HandbookCommands",handbook,"01 / INSTRUCTION STAMPS\n\n↰ LEFT / ↱ RIGHT   Turn 90°\n↶ REVERSE   Turn back\nⅡ WAIT   Pause one cycle\n>> FAST   Next move: one cycle\n~ SLOW   Next move: three cycles\n⌃ JUMP   Cross one small gap",new Vector2(-242,42),new Vector2(438,370),25);
            Label("HandbookRules",handbook,"02 / INK & PARCELS\n\n1 STANDARD   Execute once\n×2 DOUBLE   Execute twice\nLOCK PERMANENT   Reset to change\n\n! FRAGILE   Avoid bumps\n>> EXPRESS   Never wait\nR REGISTERED   Visit every check",new Vector2(242,42),new Vector2(438,370),25);
            Label("HandbookTiming",handbook,"A normal step takes 2 cycles. Gates show open phase / period.\nStamp a marked space, then Dispatch. Review and adjust at any time.",new Vector2(0,-210),new Vector2(940,86),24,Muted,TextAnchor.MiddleCenter);
            Button("Close",handbook,"BACK TO THE DESK",new Vector2(0,-313),new Vector2(470,62));
            var credits=Panel("CreditsPanel").transform;
            Label("CreditsTitle",credits,"POSTMARK PROTOCOL",new Vector2(0,278),new Vector2(960,55),36);
            Label("CreditsBody",credits,"Stamp the instructions. Send the route.\n\nDevelopment • Rehman Mughal\nOriginal project-generated sound design and music.\nOriginal scene geometry and materials.\nOriginal app artwork created with OpenAI image generation.\n\nMade with Unity. Unity trademarks belong to Unity Technologies.\nAll 36 deliveries work offline. No ads. No tracking.",new Vector2(0,25),new Vector2(920,415),27,Dark,TextAnchor.MiddleCenter);
            Button("BackSettings",credits,"BACK TO SETTINGS",new Vector2(0,-313),new Vector2(430,62));
        }
        static void Slider(Transform parent,string name,string caption,float y,float value)
        {
            Label(name+"Caption",parent,caption,new Vector2(-252,y),new Vector2(370,64),28);
            Label(name=="MusicVolume" ? "MusicValue" : "SoundValue",parent,Mathf.RoundToInt(value*100)+"%",new Vector2(421,y),new Vector2(85,64),25,Dark,TextAnchor.MiddleRight);
            var rect=Rect(name,parent,new Vector2(140,y),new Vector2(430,64));var slider=rect.gameObject.AddComponent<UnityEngine.UI.Slider>();
            var track=Image("Track",rect,Vector2.zero,new Vector2(410,10),new Color(.69f,.7f,.61f));track.raycastTarget=true;
            var fillArea=Rect("Fill area",rect,Vector2.zero,new Vector2(390,10));
            var fill=Image("Fill",fillArea,Vector2.zero,Vector2.zero,Red);Full((RectTransform)fill.transform);
            var handleArea=Rect("Handle area",rect,Vector2.zero,new Vector2(390,64));
            var handle=Image("Handle",handleArea,Vector2.zero,new Vector2(36,56),Dark);handle.raycastTarget=true;
            slider.fillRect=(RectTransform)fill.transform;slider.handleRect=(RectTransform)handle.transform;slider.targetGraphic=handle;slider.value=value;
        }
        static void MainDesk()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=Environment("MainDesk",out _,out _,out _);root.Find("Environment/RoutePaper").gameObject.SetActive(false);UI(root);
            var menu=Panel("MainMenu",false).transform;
            Label("Masthead",menu,"POSTMARK\nPROTOCOL",new Vector2(-265,224),new Vector2(540,180),72,Cream);
            Label("Tagline",menu,"Stamp the instructions. Send the route.",new Vector2(-270,98),new Vector2(530,64),26,Cream);
            Label("Edition",menu,"AN OFFLINE PUZZLE IN 36 DELIVERIES",new Vector2(-260,358),new Vector2(550,40),20,new Color(.86f,.7f,.45f));
            var start=Button("StartShift",menu,"",new Vector2(-257,-91),new Vector2(520,250),Cream);
            start.transform.localRotation=Quaternion.Euler(0,0,-3);
            Label("EnvelopeMicro",start.transform,"POSTAL SERVICE  /  OUTGOING",new Vector2(0,79),new Vector2(455,36),18,Muted);
            Label("StartShiftLabel",start.transform,"START SHIFT  →",new Vector2(0,4),new Vector2(455,65),39,Dark);
            Label("EnvelopeSub",start.transform,"Your next delivery is waiting.",new Vector2(0,-65),new Vector2(455,38),23,Muted);
            Image("Envelope postage",start.transform,new Vector2(194,79),new Vector2(32,40),Red);
            var ledger=Button("Ledger",menu,"",new Vector2(313,168),new Vector2(420,250),Dark);ledger.transform.localRotation=Quaternion.Euler(0,0,2);
            Image("Ledger spine",ledger.transform,new Vector2(-180,0),new Vector2(6,220),new Color(.69f,.53f,.3f));
            Label("LedgerFoil",ledger.transform,"DELIVERY RECORDS",new Vector2(12,68),new Vector2(340,42),18,new Color(.83f,.68f,.42f),TextAnchor.MiddleCenter);
            Label("LedgerMenuLabel",ledger.transform,"LEVEL\nLEDGER",new Vector2(12,-8),new Vector2(340,130),44,Cream,TextAnchor.MiddleCenter);
            Button("Handbook",menu,"THE HANDBOOK  ↗",new Vector2(316,-70),new Vector2(410,112),Cream,null,29);
            Button("Settings",menu,"DESK SETTINGS",new Vector2(316,-220),new Vector2(410,88),Dark);
            Label("ShiftProgress",menu,"0 / 36 DELIVERIES FILED",new Vector2(-255,-286),new Vector2(520,48),22,Cream);
            Label("Footer",menu,"REHMAN MUGHAL     /     EST. 2026",new Vector2(0,-385),new Vector2(1060,42),18,new Color(.78f,.7f,.55f),TextAnchor.MiddleCenter);
            var chapters=Panel("ChapterPanel").transform;
            Label("ChapterTitle",chapters,"THE DELIVERY LEDGER",new Vector2(0,278),new Vector2(960,55),36);
            string[] names={"LOCAL ROUTE","EXPRESS DESK","RETURN OFFICE","INK LAB","PRIORITY SERVICE","NIGHT SHIFT"};
            for(int i=0;i<6;i++) Button("Chapter_"+i,chapters,$"{i+1:00}   {names[i]}\nOPEN LEDGER  →",new Vector2(i%2==0 ? -245 : 245,150-(i/2)*151),new Vector2(450,120),i==5 ? Dark : Muted,"ChapterLabel_"+i,24);
            Button("BackMain",chapters,"BACK TO DESK",new Vector2(0,-313),new Vector2(400,62));
            var ledgerPanel=Panel("LevelLedger").transform;
            Label("LedgerTitle",ledgerPanel,"CHAPTER 01 / LOCAL ROUTE",new Vector2(0,278),new Vector2(960,55),33);
            for(int i=0;i<6;i++) Button("Level_"+i,ledgerPanel,$"{i+1:00}   DELIVERY\nAWAITING DISPATCH",new Vector2(0,188-i*80),new Vector2(954,67),i%2==0 ? Dark : Muted,"LevelLabel_"+i,21);
            Button("BackChapters",ledgerPanel,"ALL CHAPTERS",new Vector2(0,-313),new Vector2(400,62));
            CommonPanels();SaveNotice();ui.panels=panels.ToArray();foreach(var p in panels) p.SetActive(p.name=="MainMenu");
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/MainDesk.unity");
        }
        static void GameplayScene()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=Environment("Gameplay",out var camera,out var light,out var desk);UI(root);
            var presentation=root.Find("Systems").gameObject.AddComponent<RoutePresentation>();ui.route=presentation;
            presentation.routeRoot=Empty("Route",root).transform;presentation.routeRoot.position=new Vector3(-1.35f,.13f,.3f);
            presentation.cellPrefab=AssetDatabase.LoadAssetAtPath<RouteCell>("Assets/Prefabs/Gameplay/RouteCell.prefab");presentation.connectorPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/RouteLink.prefab").transform;
            presentation.parcel=(PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/Letter.prefab"),root) as GameObject).transform;presentation.parcel.name="Parcel";
            var mailbox=Empty("Mailbox",root.Find("Environment")).transform;presentation.mailbox=mailbox;
            Shape("Delivery box",mailbox,new Vector3(0,.15f,0),new Vector3(.86f,.4f,.86f),ink);
            var lid=Empty("Hinged lid",mailbox).transform;lid.localPosition=new Vector3(0,.38f,.37f);Shape("Lid",lid,new Vector3(0,0,-.37f),new Vector3(.91f,.07f,.85f),red);presentation.mailboxLid=lid;
            WorldText("Mailbox insignia",lid,"POST",new Vector3(0,.048f,-.37f),.038f,Cream);
            presentation.impactStamp=Stamp("ImpactStamp",Empty("Effects",root).transform,Vector3.zero);presentation.impactStamp.gameObject.SetActive(false);
            presentation.deliveryImprint=WorldText("Delivered ink impression",root.Find("Effects"),"DELIVERED",Vector3.zero,.35f,Red).transform;presentation.deliveryImprint.gameObject.SetActive(false);
            presentation.gameCamera=camera;presentation.deskLight=light;presentation.dayDesk=wood;presentation.nightDesk=night;presentation.deskRenderer=desk;
            var hud=Panel("GameHUD",false).transform;
            var top=Image("TopHUD",hud,new Vector2(0,-64),new Vector2(0,126),Dark);var topRect=(RectTransform)top.transform;topRect.anchorMin=new Vector2(0,1);topRect.anchorMax=new Vector2(1,1);topRect.offsetMin=new Vector2(24,-140);topRect.offsetMax=new Vector2(-24,-16);
            var chapterLabel=Label("ChapterName",top.transform,"LOCAL ROUTE",new Vector2(32,-23),new Vector2(700,32),19,new Color(.82f,.68f,.44f));AnchorLeftTop((RectTransform)chapterLabel.transform);
            var title=Label("LevelTitle",top.transform,"01 / FIRST DISPATCH",new Vector2(32,-70),new Vector2(720,56),32,Cream);AnchorLeftTop((RectTransform)title.transform);
            var pause=Button("Pause",top.transform,"PAUSE",new Vector2(-90,0),new Vector2(136,70),Cream);AnchorRight((RectTransform)pause.transform);
            var tray=Image("CommandTray",hud,new Vector2(-144,-7),new Vector2(240,580),Dark);AnchorRight((RectTransform)tray.transform);
            Label("TrayTitle",tray.transform,"INSTRUCTION\nSTAMPS",new Vector2(0,238),new Vector2(208,70),25,Cream,TextAnchor.MiddleCenter);
            string[] commands={"Left","Right","Wait","Reverse","Fast","Slow","Jump"};string[] marks={"↰","↱","Ⅱ","↶",">>","~","^"};
            for(int i=0;i<7;i++)
            {
                var button=Button("Stamp_"+commands[i],tray.transform,marks[i]+"   "+commands[i].ToUpperInvariant(),new Vector2(0,155-i*55),new Vector2(206,52),Muted,null,23);
                var drag=button.gameObject.AddComponent<StampDrag>();drag.command=commands[i];
                if(i==1) { var outline=button.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.93f,.7f,.28f);outline.effectDistance=new Vector2(4,4); }
            }
            Button("Erase",tray.transform,"LIFT INK",new Vector2(0,-246),new Vector2(206,52),Cream,null,22);
            var footer=Image("Planning slip",hud,new Vector2(0,68),new Vector2(0,125),Cream);var footerRect=(RectTransform)footer.transform;footerRect.anchorMin=new Vector2(0,0);footerRect.anchorMax=new Vector2(1,0);footerRect.offsetMin=new Vector2(24,18);footerRect.offsetMax=new Vector2(-286,145);
            var count=Label("StampCount",footer.transform,"0 / 3 STAMPS • PAR 1",new Vector2(20,-25),new Vector2(600,35),21);AnchorLeftTop((RectTransform)count.transform);
            var selection=Label("SelectionLabel",footer.transform,"RIGHT / STANDARD",new Vector2(20,-65),new Vector2(700,35),23);AnchorLeftTop((RectTransform)selection.transform);
            var clock=Label("RouteClock",footer.transform,"READY TO DISPATCH",new Vector2(20,-102),new Vector2(700,28),19,Muted);AnchorLeftTop((RectTransform)clock.transform);
            var dispatch=Button("Dispatch",hud,"DISPATCH\n↓",new Vector2(-144,83),new Vector2(240,126),Red,null,31);var dr=(RectTransform)dispatch.transform;dr.anchorMin=dr.anchorMax=new Vector2(1,0);
            var dispatchOutline=dispatch.gameObject.AddComponent<Outline>();dispatchOutline.effectColor=new Color(.93f,.7f,.28f);dispatchOutline.effectDistance=new Vector2(4,4);dispatchOutline.enabled=false;
            var lesson=Label("Lesson",hud,"Pick up the Turn Right stamp.",new Vector2(-135,-274),new Vector2(760,44),22,Cream,TextAnchor.MiddleCenter);
            var parcel=Label("ParcelLabel",hud,"STANDARD MAIL",new Vector2(-135,250),new Vector2(450,45),22,Dark,TextAnchor.MiddleCenter);
            var inkControls=Rect("InkPads",hud,new Vector2(-152,-220),new Vector2(600,60));
            Button("InkStandard",inkControls,"1 STANDARD",new Vector2(-196,0),new Vector2(184,55),Dark,null,20);
            Button("InkDouble",inkControls,"×2 DOUBLE",Vector2.zero,new Vector2(184,55),Red,null,20);
            Button("InkPermanent",inkControls,"LOCK INK",new Vector2(196,0),new Vector2(184,55),Muted,null,20);
            var pausePanel=Panel("PausePanel").transform;
            Image("Clipboard clip",pausePanel,new Vector2(0,370),new Vector2(200,48),Muted);
            Label("PauseTitle",pausePanel,"ROUTE ON HOLD",new Vector2(0,278),new Vector2(960,55),36);
            Button("Resume",pausePanel,"RESUME ROUTE",new Vector2(0,144),new Vector2(720,80));
            Button("Restart",pausePanel,"RESTART DELIVERY",new Vector2(0,42),new Vector2(720,80));
            Button("Handbook",pausePanel,"INSTRUCTIONS",new Vector2(-184,-61),new Vector2(350,80));
            Button("Settings",pausePanel,"SETTINGS",new Vector2(184,-61),new Vector2(350,80));
            Button("Desk",pausePanel,"RETURN TO DESK",new Vector2(0,-164),new Vector2(720,80),Red);
            var success=Panel("SuccessPanel").transform;
            Label("ResultTitle",success,"DELIVERED",new Vector2(0,204),new Vector2(940,105),72,Red,TextAnchor.MiddleCenter);
            Label("ResultReason",success,"Another letter, safely home.",new Vector2(0,115),new Vector2(930,70),27,Dark,TextAnchor.MiddleCenter);
            ui.rewardSeals=new GameObject[3];
            string[] sealNames={"DELIVERED","EFFICIENT","PERFECT\nROUTE"};
            for(int i=0;i<3;i++)
            {
                var seal=Image("Postal seal "+i,success,new Vector2((i-1)*294,-14),new Vector2(268,128),i==2 ? Dark : Red);
                Image("Seal paper center",seal.transform,Vector2.zero,new Vector2(256,116),Cream);
                Label("Seal legend "+i,seal.transform,sealNames[i],Vector2.zero,new Vector2(230,100),28,i==2 ? Dark : Red,TextAnchor.MiddleCenter);
                seal.transform.localRotation=Quaternion.Euler(0,0,i==0 ? -5 : i==1 ? 3 : -2);ui.rewardSeals[i]=seal.gameObject;
            }
            Label("ResultDetails",success,"",new Vector2(0,-129),new Vector2(940,52),25,Muted,TextAnchor.MiddleCenter);
            Button("Next",success,"NEXT DELIVERY  →",new Vector2(0,-218),new Vector2(720,76),Red,"NextLabel");
            Button("Review",success,"REPLAY ROUTE",new Vector2(-231,-313),new Vector2(436,62));Button("Desk",success,"RETURN TO DESK",new Vector2(231,-313),new Vector2(436,62));
            var failure=Panel("FailurePanel").transform;
            Label("FailureTitle",failure,"RETURNED",new Vector2(0,180),new Vector2(940,116),72,Red,TextAnchor.MiddleCenter);
            Label("FailureReason",failure,"Route blocked.",new Vector2(0,49),new Vector2(890,110),32,Dark,TextAnchor.MiddleCenter);
            Label("FailureAdvice",failure,"Every route can be corrected. Your stamps are still on the sheet.",new Vector2(0,-66),new Vector2(890,90),27,Muted,TextAnchor.MiddleCenter);
            Button("TryAgain",failure,"TRY AGAIN",new Vector2(-235,-214),new Vector2(440,80),Red);
            Button("Review",failure,"REVIEW ROUTE",new Vector2(235,-214),new Vector2(440,80));
            Button("Desk",failure,"RETURN TO DESK",new Vector2(0,-313),new Vector2(420,62));
            CommonPanels();
            var ghost=Image("Physical stamp drag",safe,Vector2.zero,new Vector2(154,126),Dark);var sh=ghost.gameObject.AddComponent<Shadow>();sh.effectDistance=new Vector2(14,-22);sh.effectColor=new Color(0,0,0,.45f);
            Image("Wood grip",ghost.transform,new Vector2(0,35),new Vector2(92,27),new Color(.43f,.27f,.15f));
            Label("GhostLabel",ghost.transform,"RIGHT",new Vector2(0,-10),new Vector2(145,62),25,Cream,TextAnchor.MiddleCenter);ui.stampGhost=(RectTransform)ghost.transform;ghost.gameObject.SetActive(false);
            SaveNotice();ui.panels=panels.ToArray();foreach(var p in panels) p.SetActive(p.name=="GameHUD");
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Gameplay.unity");
        }
        static void AnchorLeftTop(RectTransform rect) { rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,.5f); }
        static void AnchorRight(RectTransform rect) { rect.anchorMin=rect.anchorMax=new Vector2(1,.5f); }
        static void SaveNotice() { var label=Label("SaveNotice",safe,"",new Vector2(0,12),new Vector2(1000,32),20,Red,TextAnchor.MiddleCenter);var r=(RectTransform)label.transform;r.anchorMin=r.anchorMax=new Vector2(.5f,0); }
    }
}
