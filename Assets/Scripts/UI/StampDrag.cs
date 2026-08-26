using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RehmanMughal.PostmarkProtocol.UI
{
    public sealed class StampDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public string command;
        private SceneUI ui;
        private void Awake() { ui = GetComponentInParent<SceneUI>(); }
        public void OnBeginDrag(PointerEventData e)
        {
            if (!GetComponent<Button>().interactable) return;
            ui.SelectStamp(command);
            ui.DragStamp(e.position, true);
        }
        public void OnDrag(PointerEventData e) { if (GetComponent<Button>().interactable) ui.DragStamp(e.position, true); }
        public void OnEndDrag(PointerEventData e) { ui.DropStamp(e.position); }
    }
}
