using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    public static class VisualQA
    {
        public static string Resolution(int width,int height)
        {
            var assembly=typeof(UnityEditor.Editor).Assembly;
            var sizesType=assembly.GetType("UnityEditor.GameViewSizes");
            var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var sizes=singleton.GetProperty("instance",BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy).GetValue(null);
            var group=sizesType.GetProperty("currentGroup",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).GetValue(sizes);
            var sizeType=assembly.GetType("UnityEditor.GameViewSize");
            var kind=assembly.GetType("UnityEditor.GameViewSizeType");
            var size=Activator.CreateInstance(sizeType,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,new object[]{Enum.Parse(kind,"FixedResolution"),width,height,"Postmark QA "+width+"x"+height},null);
            group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});
            int index=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null)-1;
            var view=EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));
            view.GetType().GetProperty("selectedSizeIndex",BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).SetValue(view,index);
            // Unattended automation can leave the Game View undocked/unfocused, in which case Screen.width/height
            // keeps tracking whatever size it last actually rendered at instead of this selection. Focus forces it.
            view.Focus();
            view.Repaint();
            return "Game View requested "+width+"x"+height;
        }
        public static string LayoutAudit()
        {
            foreach(var safe in UnityEngine.Object.FindObjectsByType<RehmanMughal.PostmarkProtocol.UI.SafeArea>(FindObjectsSortMode.None)) safe.Apply();
            Canvas.ForceUpdateCanvases();
            var failures=new System.Collections.Generic.List<string>();
            foreach(var label in UnityEngine.Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if(string.IsNullOrWhiteSpace(label.text)) continue;
                // Scroll content intentionally extends beyond its clipped viewport.
                if(label.GetComponentInParent<ScrollRect>()!=null) continue;
                float preferred=label.preferredHeight;
                if(label.resizeTextForBestFit)
                {
                    var settings=label.GetGenerationSettings(label.rectTransform.rect.size);
                    settings.resizeTextForBestFit=false;
                    settings.fontSize=label.cachedTextGenerator.fontSizeUsedForBestFit;
                    preferred=label.cachedTextGeneratorForLayout.GetPreferredHeight(label.text,settings)/label.pixelsPerUnit;
                }
                if(preferred>label.rectTransform.rect.height+1) failures.Add(label.name+" text height "+preferred+" > "+label.rectTransform.rect.height);
                var corners=new Vector3[4];label.rectTransform.GetWorldCorners(corners);
                if(corners.Any(p=>p.x < -1 || p.y < -1 || p.x > Screen.width+1 || p.y > Screen.height+1)) failures.Add(label.name+" outside screen");
            }
            foreach(var control in UnityEngine.Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None))
            {
                var corners=new Vector3[4];((RectTransform)control.transform).GetWorldCorners(corners);
                if(corners.Any(p=>p.x < -1 || p.y < -1 || p.x > Screen.width+1 || p.y > Screen.height+1)) failures.Add(control.name+" control outside screen");
                if(control is Slider slider && slider.handleRect!=null)
                {
                    var handleCorners=new Vector3[4];slider.handleRect.GetWorldCorners(handleCorners);
                    if(handleCorners[0].y<corners[0].y-1 || handleCorners[2].y>corners[2].y+1) failures.Add(control.name+" handle extends outside its hit area");
                }
            }
            string result=$"{Screen.width}x{Screen.height}: "+(failures.Count==0 ? "PASS text extents and screen bounds" : string.Join("; ",failures));
            Directory.CreateDirectory("QA");File.AppendAllText("QA/LayoutAudit.txt",DateTime.Now.ToString("s")+" "+result+Environment.NewLine);
            return result;
        }
        public static string Capture(string name)
        {
            Directory.CreateDirectory("QA/Screens");
            string path=Path.GetFullPath("QA/Screens/"+name+".png");ScreenCapture.CaptureScreenshot(path);return path;
        }
    }
}
