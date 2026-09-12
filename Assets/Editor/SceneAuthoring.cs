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
            if(paperPanel)
            {
                rect.gameObject.AddComponent<PanelMotion>();
                var scrim=Image("Desk dimmer",rect,Vector2.zero,Vector2.zero,new Color(.07f,.12f,.1f,.78f));Full((RectTransform)scrim.transform);scrim.raycastTarget=true;
                Image("Cotton paper panel",rect,Vector2.zero,new Vector2(936,524),Cream);
                Image("Letterhead rule",rect,new Vector2(0,170),new Vector2(872,2),Muted);
                Label("Panel running header",rect,"POSTMARK PROTOCOL / POSTAL SERVICE",new Vector2(0,239),new Vector2(864,26),20,Muted);
            }
            return rect.gameObject;
        }
        static void Title(Transform panel,string name,string caption)
        { Label(name,panel,caption,new Vector2(0,202),new Vector2(864,44),30); }
        static void UI(Transform root)
        {
            panels.Clear();
            var canvas=Empty("UI",root).AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.referenceResolution=new Vector2(960,540);
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            ui=canvas.gameObject.AddComponent<SceneUI>();
            safe=Rect("SafeArea",canvas.transform,Vector2.zero,Vector2.zero);Full((RectTransform)safe);safe.gameObject.AddComponent<SafeArea>();
            var audio=Empty("Audio").AddComponent<AudioService>();AudioAuthoring.Configure(audio);
            var eventSystem=Empty("EventSystem",root);eventSystem.AddComponent<EventSystem>();eventSystem.AddComponent<InputSystemUIInputModule>();
            ui.levels=AssetDatabase.FindAssets("t:LevelDefinition").Select(g=>AssetDatabase.LoadAssetAtPath<LevelDefinition>(AssetDatabase.GUIDToAssetPath(g))).OrderBy(l=>l.id).ToArray();
            ui.appInfo=AssetDatabase.LoadAssetAtPath<AppInformation>("Assets/Data/AppInformation.asset");
        }
        static void ScrollText(Transform panel,string name,string value)
        {
            var r=Rect(name+"Scroll",panel,new Vector2(0,23),new Vector2(864,272));
            var bg=r.gameObject.AddComponent<Image>();bg.color=Cream;bg.raycastTarget=true;
            var scroll=r.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;
            var viewport=Rect("Viewport",r,Vector2.zero,Vector2.zero);Full(viewport);viewport.gameObject.AddComponent<RectMask2D>();
            var text=Label(name,viewport,value,Vector2.zero,new Vector2(840,600),24,Dark,TextAnchor.UpperLeft);
            var content=text.rectTransform;content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.offsetMin=new Vector2(8,0);content.offsetMax=new Vector2(-20,0);
            var fitter=text.gameObject.AddComponent<ContentSizeFitter>();fitter.verticalFit=ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport=viewport;scroll.content=content;scroll.verticalNormalizedPosition=1;
            Label(name+"ScrollHint",panel,"SWIPE UP TO READ MORE",new Vector2(0,-134),new Vector2(840,26),20,Muted,TextAnchor.MiddleCenter);
        }
        static void CommonPanels()
        {
            var settings=Panel("SettingsPanel").transform;Title(settings,"SettingsTitle","DESK SETTINGS");
            Slider(settings,"MusicVolume","Music",118,SaveService.Current.musicVolume);
            Slider(settings,"SoundVolume","Sound effects",36,SaveService.Current.soundVolume);
            var toggleRect=Rect("Haptics",settings,new Vector2(251,-46),new Vector2(330,80));var toggle=toggleRect.gameObject.AddComponent<Toggle>();
            var bg=Image("Toggle track",toggleRect,Vector2.zero,new Vector2(330,80),Dark);bg.raycastTarget=true;toggle.targetGraphic=bg;
            var check=Image("Haptics check",toggleRect,new Vector2(-121,0),new Vector2(36,36),Red);toggle.graphic=check;
            Label("HapticsOnLabel",toggleRect,"ENABLED",new Vector2(23,0),new Vector2(220,50),24,Cream,TextAnchor.MiddleCenter);
            Label("HapticsLabel",settings,"Haptic feedback",new Vector2(-252,-46),new Vector2(360,64),25);
            // Below Haptics there is only ~176 units left before the canvas' own -270 edge (not the paper's, which is decorative and can hang past it); three rows have to fit there, so these four buttons run shorter (52 tall, size 20 font) than the sliders/toggle above. Reset/Close are widened to 876 to span the same width as the Privacy+Support pair instead of looking narrower than the row above them.
            Button("PrivacyPolicy",settings,"PRIVACY POLICY",new Vector2(-223,-126),new Vector2(430,52),null,null,20);
            Button("Support",settings,"SUPPORT",new Vector2(223,-126),new Vector2(430,52),null,null,20);
            Button("ResetProgress",settings,"RESET PROGRESS",new Vector2(0,-182),new Vector2(876,52),Red,null,20);
            Button("Close",settings,"CLOSE SETTINGS",new Vector2(0,-238),new Vector2(876,52),null,null,20);
            var reset=Panel("ResetDialog").transform;Title(reset,"ResetTitle","CLEAR THE LEDGER?");
            Label("ResetExplanation",reset,"Clear delivery progress, seals and tutorial state.\nMusic, sound and haptic preferences are kept.\nThis cannot be undone.",new Vector2(0,78),new Vector2(840,152),26,Dark,TextAnchor.MiddleCenter);
            Label("ResetStatus",reset,"",new Vector2(0,-68),new Vector2(840,128),25,Red,TextAnchor.MiddleCenter);
            Button("CancelReset",reset,"BACK TO SETTINGS",new Vector2(-223,-221),new Vector2(430,80));
            Button("ConfirmReset",reset,"CLEAR PROGRESS",new Vector2(223,-221),new Vector2(430,80),Red);
            var intro=Panel("IntroPanel").transform;Title(intro,"IntroTitle","WELCOME TO THE DESK");
            Label("IntroBody",intro,"You don't steer the letter — you program it.\n\nStamp your instructions on the route, then press Dispatch.",new Vector2(0,32),new Vector2(840,240),30,Dark,TextAnchor.MiddleCenter);
            Button("GotIt",intro,"GOT IT",new Vector2(0,-221),new Vector2(430,80));
            var handbook=Panel("HandbookPanel").transform;Title(handbook,"HandbookTitle","THE ROUTE HANDBOOK");
            ScrollText(handbook,"HandbookBody","INSTRUCTION STAMPS\nLEFT / RIGHT: Turn 90 degrees. REVERSE: Turn back.\nWAIT: Pause one cycle. FAST: Next move one cycle. SLOW: Next move three cycles. JUMP: Cross a small marked gap.\n\nINK AND PARCELS\nSTANDARD executes once per visit. DOUBLE executes twice per visit. PERMANENT locks editing until Restart. Stamps remain on the sheet through a dispatch.\nFRAGILE: Avoid bumps. EXPRESS: Never wait. REGISTERED: Visit every checkpoint.\n\nPLANNING\nChoose a stamp, then tap a marked space. You can also drag a stamp onto the route. LIFT INK erases a non-permanent stamp. From chapter 4, choose INK to open the ink selector.\n\nTIMING AND SEALS\nA normal step takes two cycles. Gates show open phase / period. Dispatch runs your plan. Pause stops it. Review lets you adjust it.\nDelivered: Reach the mailbox. Efficient: Meet par stamp count. Perfect Route: Meet par and optimal distance.");
            Button("Close",handbook,"BACK",new Vector2(0,-221),new Vector2(430,80));
            var privacy=Panel("PrivacyPanel").transform;Title(privacy,"PrivacyTitle","PRIVACY POLICY");
            ScrollText(privacy,"PrivacyBody","NO DATA COLLECTED\nPostmark Protocol does not collect, store or transmit any personal information. The game works fully offline and never needs an account, a login, or an internet connection.\n\nLOCAL SAVE FILE\nYour delivery progress, unlocked levels, best results, and your music, sound and haptic preferences are saved only in a file on your own device. This information never leaves your device.\n\nNO THIRD PARTIES\nThis app contains no advertising, no analytics, no tracking and no in app purchases. Nothing is ever shared with a third party because nothing is collected.\n\nCHILDREN\nNo personal information is collected from any player, including children, so no additional data safeguards are required.\n\nCONTACT\nQuestions about this policy can be sent to buistovanhl@icloud.com.\n\nThis policy may be updated occasionally; the date above reflects the latest revision.");
            Button("Close",privacy,"BACK",new Vector2(0,-221),new Vector2(430,80));
        }
        static void Slider(Transform parent,string name,string caption,float y,float value)
        {
            Label(name+"Caption",parent,caption,new Vector2(-252,y),new Vector2(360,64),25);
            Label(name=="MusicVolume" ? "MusicValue" : "SoundValue",parent,Mathf.RoundToInt(value*100)+"%",new Vector2(392,y),new Vector2(80,64),24,Dark,TextAnchor.MiddleRight);
            var rect=Rect(name,parent,new Vector2(174,y),new Vector2(320,80));var slider=rect.gameObject.AddComponent<UnityEngine.UI.Slider>();
            var hit=Image("Slider hit area",rect,Vector2.zero,new Vector2(320,80),Color.clear);hit.raycastTarget=true;
            Image("Track",rect,Vector2.zero,new Vector2(270,8),new Color(.69f,.7f,.61f));
            var fillArea=Rect("Fill area",rect,Vector2.zero,new Vector2(270,8));
            var fill=Image("Fill",fillArea,Vector2.zero,Vector2.zero,Red);Full((RectTransform)fill.transform);
            var handleArea=Rect("Handle area",rect,Vector2.zero,new Vector2(270,56));
            // Slider stretches the handle vertically; zero sizeDelta avoids adding height twice.
            var handle=Image("Handle",handleArea,Vector2.zero,new Vector2(40,0),Dark);handle.raycastTarget=true;
            slider.fillRect=(RectTransform)fill.transform;slider.handleRect=(RectTransform)handle.transform;slider.targetGraphic=handle;
            slider.maxValue=100;slider.wholeNumbers=true;slider.value=Mathf.RoundToInt(value*100);
        }
        static void FinishScene(UnityEngine.SceneManagement.Scene scene,string name,string initial)
        {
            SaveNotice();ui.panels=panels.ToArray();foreach(var p in panels) p.SetActive(p.name==initial);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/"+name+".unity");
        }
        static void MainDesk()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=Environment("MainDesk",out _,out _,out _);root.Find("Environment/RoutePaper").gameObject.SetActive(false);UI(root);
            var menu=Panel("MainMenu",false).transform;
            Label("Masthead",menu,"POSTMARK\nPROTOCOL",new Vector2(-222,135),new Vector2(444,152),54,Cream);
            Label("Tagline",menu,"Stamp the instructions. Send the route.",new Vector2(-215,24),new Vector2(430,64),24,Cream);
            var start=Button("StartShift",menu,"START SHIFT  →",new Vector2(-217,-93),new Vector2(430,118),Cream,null,34);
            Label("ShiftProgress",menu,"0 / 36 DELIVERIES FILED",new Vector2(-217,-190),new Vector2(430,44),23,Cream);
            Button("Ledger",menu,"DELIVERY LEDGER",new Vector2(247,117),new Vector2(400,120),Dark,null,30);
            Button("Handbook",menu,"THE HANDBOOK",new Vector2(247,-5),new Vector2(400,96),Cream,null,28);
            Button("Settings",menu,"DESK SETTINGS",new Vector2(247,-115),new Vector2(400,96));
            var chapters=Panel("ChapterPanel").transform;Title(chapters,"ChapterTitle","THE DELIVERY LEDGER");
            string[] names={"LOCAL ROUTE","EXPRESS DESK","RETURN OFFICE","INK LAB","PRIORITY SERVICE","NIGHT SHIFT"};
            for(int i=0;i<6;i++) Button("Chapter_"+i,chapters,$"{i+1:00}   {names[i]}\nOPEN LEDGER",new Vector2(i%2==0 ? -223 : 223,108-(i/2)*96),new Vector2(430,88),i==5 ? Dark : Muted,"ChapterLabel_"+i,24);
            Button("BackMain",chapters,"BACK TO DESK",new Vector2(0,-221),new Vector2(430,80));
            var ledger=Panel("LevelLedger").transform;Title(ledger,"LedgerTitle","CHAPTER 01 / LOCAL ROUTE");
            for(int i=0;i<6;i++) Button("Level_"+i,ledger,$"{i+1:00}   DELIVERY\nAWAITING DISPATCH",new Vector2(i%2==0 ? -223 : 223,108-(i/2)*96),new Vector2(430,88),i%2==0 ? Dark : Muted,"LevelLabel_"+i,21);
            Button("BackChapters",ledger,"ALL CHAPTERS",new Vector2(0,-221),new Vector2(430,80));
            CommonPanels();FinishScene(scene,"MainDesk","MainMenu");
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
            presentation.deliveryImprint=WorldText("Delivered ink impression",root.Find("Effects"),"DELIVERED",Vector3.zero,.13f,Red).transform;presentation.deliveryImprint.gameObject.SetActive(false);
            presentation.gameCamera=camera;presentation.deskLight=light;presentation.dayDesk=wood;presentation.nightDesk=night;presentation.deskRenderer=desk;
            var hud=Panel("GameHUD",false).transform;
            var top=Image("TopHUD",hud,Vector2.zero,Vector2.zero,Dark);var tr=(RectTransform)top.transform;Full(tr);tr.anchorMin=new Vector2(0,1);tr.offsetMin=new Vector2(12,-94);tr.offsetMax=new Vector2(-12,-10);
            AnchorLeftTop(Label("ChapterName",top.transform,"LOCAL ROUTE",new Vector2(20,-19),new Vector2(620,26),20,new Color(.82f,.68f,.44f)).rectTransform);
            AnchorLeftTop(Label("LevelTitle",top.transform,"01 / FIRST CLASS",new Vector2(20,-56),new Vector2(620,42),28,Cream).rectTransform);
            var pause=Button("Pause",top.transform,"PAUSE",new Vector2(-74,0),new Vector2(136,80),Cream);AnchorRight((RectTransform)pause.transform);
            var tray=Image("CommandTray",hud,new Vector2(-172,0),new Vector2(320,336),Dark);AnchorRight((RectTransform)tray.transform);
            string[] commands={"Left","Right","Wait","Reverse","Fast","Slow","Jump"};
            for(int i=0;i<7;i++)
            {
                var button=Button("Stamp_"+commands[i],tray.transform,commands[i].ToUpperInvariant(),new Vector2(i%2==0 ? -78 : 78,126-(i/2)*84),new Vector2(148,80),Muted,null,23);
                var drag=button.gameObject.AddComponent<StampDrag>();drag.command=commands[i];
                if(i==1) { var outline=button.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.93f,.7f,.28f);outline.effectDistance=new Vector2(3,3); }
            }
            Button("Erase",tray.transform,"LIFT INK",new Vector2(78,-126),new Vector2(148,80),Cream,null,22);
            var footer=Image("Planning slip",hud,Vector2.zero,Vector2.zero,Cream);var fr=(RectTransform)footer.transform;Full(fr);fr.anchorMax=new Vector2(1,0);fr.offsetMin=new Vector2(12,10);fr.offsetMax=new Vector2(-344,94);
            AnchorLeftTop(Label("StampCount",footer.transform,"0 / 3 STAMPS / PAR 1",new Vector2(12,-16),new Vector2(560,26),20).rectTransform);
            AnchorLeftTop(Label("SelectionLabel",footer.transform,"RIGHT / STANDARD",new Vector2(12,-42),new Vector2(560,26),21).rectTransform);
            AnchorLeftTop(Label("RouteClock",footer.transform,"READY TO DISPATCH",new Vector2(12,-68),new Vector2(560,26),20,Muted).rectTransform);
            var dispatch=Button("Dispatch",hud,"DISPATCH",new Vector2(-224,52),new Vector2(208,84),Red,null,27);var dr=(RectTransform)dispatch.transform;dr.anchorMin=dr.anchorMax=new Vector2(1,0);
            var outlineDispatch=dispatch.gameObject.AddComponent<Outline>();outlineDispatch.effectColor=new Color(.93f,.7f,.28f);outlineDispatch.effectDistance=new Vector2(3,3);outlineDispatch.enabled=false;
            var chooseInk=Button("ChooseInk",hud,"INK",new Vector2(-62,52),new Vector2(100,84),Dark,null,24);var ir=(RectTransform)chooseInk.transform;ir.anchorMin=ir.anchorMax=new Vector2(1,0);
            var board=Rect("BoardArea",hud,Vector2.zero,Vector2.zero);Full(board);board.offsetMin=new Vector2(22,169);board.offsetMax=new Vector2(-354,-129);presentation.boardArea=board;
            var hitSurface=board.gameObject.AddComponent<Image>();hitSurface.color=Color.clear;hitSurface.raycastTarget=true;
            board.gameObject.AddComponent<RouteBoardInput>().route=presentation;
            var lessonBand=Image("Lesson band",hud,Vector2.zero,Vector2.zero,Dark).rectTransform;Full(lessonBand);lessonBand.anchorMax=new Vector2(1,0);lessonBand.offsetMin=new Vector2(12,98);lessonBand.offsetMax=new Vector2(-344,164);
            var lesson=Label("Lesson",lessonBand,"Pick up the Turn Right stamp.",Vector2.zero,Vector2.zero,22,Cream,TextAnchor.MiddleCenter).rectTransform;Full(lesson,5);
            var parcelBand=Image("Parcel band",hud,Vector2.zero,Vector2.zero,Dark).rectTransform;Full(parcelBand);parcelBand.anchorMin=new Vector2(0,1);parcelBand.offsetMin=new Vector2(12,-125);parcelBand.offsetMax=new Vector2(-344,-95);
            var parcel=Label("ParcelLabel",parcelBand,"STANDARD MAIL",Vector2.zero,Vector2.zero,20,Cream,TextAnchor.MiddleCenter).rectTransform;Full(parcel);
            var inkPanel=Panel("InkPanel").transform;Title(inkPanel,"InkTitle","CHOOSE YOUR INK");
            Label("InkHelp",inkPanel,"Standard: once per visit. Double: twice per visit.\nPermanent: locked against editing until Restart.",new Vector2(0,93),new Vector2(864,106),27,Dark,TextAnchor.MiddleCenter);
            Button("InkStandard",inkPanel,"STANDARD",new Vector2(-292,-30),new Vector2(280,100),Dark);
            Button("InkDouble",inkPanel,"DOUBLE",new Vector2(0,-30),new Vector2(280,100),Red);
            Button("InkPermanent",inkPanel,"PERMANENT",new Vector2(292,-30),new Vector2(280,100),Muted);
            Button("CancelInk",inkPanel,"BACK TO ROUTE",new Vector2(0,-221),new Vector2(430,80));
            var pausePanel=Panel("PausePanel").transform;Title(pausePanel,"PauseTitle","ROUTE ON HOLD");
            Button("Resume",pausePanel,"RESUME ROUTE",new Vector2(0,110),new Vector2(864,84));
            Button("Restart",pausePanel,"RESTART DELIVERY",new Vector2(-223,16),new Vector2(430,84));
            Button("Handbook",pausePanel,"INSTRUCTIONS",new Vector2(223,16),new Vector2(430,84));
            Button("Settings",pausePanel,"SETTINGS",new Vector2(-223,-78),new Vector2(430,84));
            Button("Desk",pausePanel,"RETURN TO DESK",new Vector2(223,-78),new Vector2(430,84),Red);
            var success=Panel("SuccessPanel").transform;
            Label("ResultTitle",success,"DELIVERED",new Vector2(0,193),new Vector2(864,64),46,Red,TextAnchor.MiddleCenter);
            Label("ResultReason",success,"Another letter, safely home.",new Vector2(0,128),new Vector2(864,62),25,Dark,TextAnchor.MiddleCenter);
            ui.rewardSeals=new GameObject[3];string[] sealNames={"DELIVERED","EFFICIENT","PERFECT\nROUTE"};
            for(int i=0;i<3;i++)
            {
                var seal=Image("Postal seal "+i,success,new Vector2((i-1)*284,27),new Vector2(256,100),i==2 ? Dark : Red);
                Image("Seal paper center",seal.transform,Vector2.zero,new Vector2(246,90),Cream);
                Label("Seal legend "+i,seal.transform,sealNames[i],Vector2.zero,new Vector2(232,78),25,i==2 ? Dark : Red,TextAnchor.MiddleCenter);
                ui.rewardSeals[i]=seal.gameObject;
            }
            Label("ResultDetails",success,"",new Vector2(0,-61),new Vector2(864,42),23,Muted,TextAnchor.MiddleCenter);
            Button("Next",success,"NEXT DELIVERY",new Vector2(0,-133),new Vector2(864,80),Red,"NextLabel");
            Button("Review",success,"REPLAY ROUTE",new Vector2(-223,-221),new Vector2(430,80));
            Button("Desk",success,"RETURN TO DESK",new Vector2(223,-221),new Vector2(430,80));
            var failure=Panel("FailurePanel").transform;Title(failure,"FailureTitle","RETURNED");
            Label("FailureReason",failure,"Route blocked.",new Vector2(0,98),new Vector2(864,102),30,Red,TextAnchor.MiddleCenter);
            Label("FailureAdvice",failure,"Every route can be corrected. Your stamps are still on the sheet.",new Vector2(0,-8),new Vector2(864,100),26,Muted,TextAnchor.MiddleCenter);
            Button("TryAgain",failure,"TRY AGAIN",new Vector2(-223,-133),new Vector2(430,80),Red);
            Button("Review",failure,"REVIEW ROUTE",new Vector2(223,-133),new Vector2(430,80));
            Button("Desk",failure,"RETURN TO DESK",new Vector2(0,-221),new Vector2(430,80));
            CommonPanels();
            var ghost=Image("Physical stamp drag",safe,Vector2.zero,new Vector2(116,100),Dark);
            Label("GhostLabel",ghost.transform,"RIGHT",Vector2.zero,new Vector2(112,68),22,Cream,TextAnchor.MiddleCenter);ui.stampGhost=(RectTransform)ghost.transform;ghost.gameObject.SetActive(false);
            FinishScene(scene,"Gameplay","GameHUD");
        }
        static void AnchorLeftTop(RectTransform rect) { rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,.5f); }
        static void AnchorRight(RectTransform rect) { rect.anchorMin=rect.anchorMax=new Vector2(1,.5f); }
        static void SaveNotice()
        {
            var label=Label("SaveNotice",safe,"",new Vector2(0,15),new Vector2(880,28),20,Red,TextAnchor.MiddleCenter);
            var r=(RectTransform)label.transform;r.anchorMin=r.anchorMax=new Vector2(.5f,0);
        }
    }
}
