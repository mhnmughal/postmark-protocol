using UnityEngine;
using UnityEngine.EventSystems;

namespace RehmanMughal.PostmarkProtocol.UI
{
    public sealed class RouteCell : MonoBehaviour, IPointerClickHandler
    {
        public MeshRenderer surface;
        public TextMesh label;
        public GameObject slotBorder;
        [System.NonSerialized] public Vector2Int position;
        [System.NonSerialized] public RoutePresentation owner;
        public void OnPointerClick(PointerEventData e) { if (owner != null) owner.ui.Place(position); }
    }
}
