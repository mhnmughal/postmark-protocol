using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RehmanMughal.PostmarkProtocol.UI
{
    public sealed class PaperMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 rest;
        private Coroutine motion;
        private void Awake() { rest = transform.localScale; }
        public void OnPointerEnter(PointerEventData data) { Move(1.025f); }
        public void OnPointerExit(PointerEventData data) { Move(1); }
        public void OnPointerDown(PointerEventData data) { Move(.96f); }
        public void OnPointerUp(PointerEventData data) { Move(1); }
        private void OnDisable() { if (rest != Vector3.zero) transform.localScale = rest; }
        private void Move(float scale)
        {
            if (!isActiveAndEnabled) return;
            if (motion != null) StopCoroutine(motion);
            motion = StartCoroutine(Ease(scale));
        }
        private IEnumerator Ease(float scale)
        {
            Vector3 from = transform.localScale;
            for (float t = 0; t < .14f; t += Time.unscaledDeltaTime)
            {
                transform.localScale = Vector3.Lerp(from, rest * scale, Mathf.SmoothStep(0, 1, t / .14f));
                yield return null;
            }
            transform.localScale = rest * scale;
        }
    }
}
