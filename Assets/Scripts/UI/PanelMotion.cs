using System.Collections;
using UnityEngine;

namespace RehmanMughal.PostmarkProtocol.UI
{
    public sealed class PanelMotion : MonoBehaviour
    {
        private void OnEnable() { if(Application.isPlaying) StartCoroutine(Slide()); }
        private IEnumerator Slide()
        {
            var rect=(RectTransform)transform;
            for(float t=0;t<.24f;t+=Time.unscaledDeltaTime)
            {
                rect.anchoredPosition=Vector2.up*Mathf.Lerp(-46,0,1-Mathf.Pow(1-t/.24f,3));
                yield return null;
            }
            rect.anchoredPosition=Vector2.zero;
        }
        private void OnDisable() { ((RectTransform)transform).anchoredPosition=Vector2.zero; }
    }
}
