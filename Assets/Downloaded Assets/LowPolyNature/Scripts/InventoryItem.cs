using System;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class InventoryEventArgs : EventArgs
    {
        public InventoryItemBase Item;

        public InventoryEventArgs(InventoryItemBase item)
        {
            Item = item;
        }
    }
}