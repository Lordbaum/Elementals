using UnityEngine;
using UnityEngine.EventSystems;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public InventoryItemBase Item { get; set; }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = Vector3.zero;
        }
    }
}