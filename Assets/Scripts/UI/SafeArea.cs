using UnityEngine;

namespace RehmanMughal.PostmarkProtocol.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        private Rect previous;
        private Vector2 size;
        private void OnEnable() { Apply(); }
        private void Update()
        {
            if (previous != Screen.safeArea || size.x != Screen.width || size.y != Screen.height) Apply();
        }
        public void Apply()
        {
            if (Screen.width == 0 || Screen.height == 0) return;
            previous = Screen.safeArea;
            if(previous.width <= 0 || previous.height <= 0) previous = new Rect(0,0,Screen.width,Screen.height);
            size = new Vector2(Screen.width, Screen.height);
            // Fit the authored 960x540 layout inside the usable area, including narrow tablets.
            var scaler = GetComponentInParent<UnityEngine.UI.CanvasScaler>();
            scaler.scaleFactor = Mathf.Min(previous.width / 960f, previous.height / 540f);
            // Apply immediately; CanvasScaler.Update can otherwise lag Screen by one Editor frame.
            scaler.GetComponent<Canvas>().scaleFactor = scaler.scaleFactor;
            var rect = (RectTransform)transform;
            rect.anchorMin = previous.position / size;
            rect.anchorMax = (previous.position + previous.size) / size;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
