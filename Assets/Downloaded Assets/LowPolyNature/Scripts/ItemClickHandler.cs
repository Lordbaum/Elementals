using UnityEngine;
using UnityEngine.UI;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class ItemClickHandler : MonoBehaviour
    {
        public Inventory _Inventory;

        public KeyCode _Key;

        private Button _button;

        private InventoryItemBase AttachedItem
        {
            get
            {
                ItemDragHandler dragHandler =
                    gameObject.transform.Find("ItemImage").GetComponent<ItemDragHandler>();

                return dragHandler.Item;
            }
        }

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        void Update()
        {
            if (Input.GetKeyDown(_Key))
            {
                FadeToColor(_button.colors.pressedColor);

                // Click the button
                _button.onClick.Invoke();
            }
            else if (Input.GetKeyUp(_Key))
            {
                FadeToColor(_button.colors.normalColor);
            }
        }

        void FadeToColor(Color color)
        {
            Graphic graphic = GetComponent<Graphic>();
            graphic.CrossFadeColor(color, _button.colors.fadeDuration, true, true);
        }

        public void OnItemClicked()
        {
            InventoryItemBase item = AttachedItem;

            if (item != null)
            {
                _Inventory.UseItem(item);
            }
        }
    }
}