using UnityEngine;
using UnityEngine.EventSystems;

namespace RehmanMughal.PostmarkProtocol.UI
{
    // Authored transparent hit surface; near a marked space, choose the nearest slot.
    public sealed class RouteBoardInput : MonoBehaviour, IPointerClickHandler
    {
        public RoutePresentation route;
        public void OnPointerClick(PointerEventData e)
        {
            if(route != null && route.ui.IsPlanning && route.TryPoint(e.position, out var cell)) route.ui.Place(cell);
        }
    }
}
