using UnityEngine;
using UnityEngine.EventSystems;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class ItemDropHandler : MonoBehaviour, IDropHandler
    {
        public Inventory _Inventory;

        public void OnDrop(PointerEventData eventData)
        {
            RectTransform invPanel = transform as RectTransform;

            if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel,
                    Input.mousePosition))
            {
                InventoryItemBase item = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>().Item;
                if (item != null)
                {
                    _Inventory.RemoveItem(item);
                    item.OnDrop();
                }
            }
        }
    }
}