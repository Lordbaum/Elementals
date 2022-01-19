using System.Collections.Generic;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class InventorySlot
    {
        private int mId = 0;
        private Stack<InventoryItemBase> mItemStack = new Stack<InventoryItemBase>();

        public InventorySlot(int id)
        {
            mId = id;
        }

        public int Id
        {
            get { return mId; }
        }

        public InventoryItemBase FirstItem
        {
            get
            {
                if (IsEmpty)
                    return null;

                return mItemStack.Peek();
            }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public int Count
        {
            get { return mItemStack.Count; }
        }

        public void AddItem(InventoryItemBase item)
        {
            item.Slot = this;
            mItemStack.Push(item);
        }

        public bool IsStackable(InventoryItemBase item)
        {
            if (IsEmpty)
                return false;

            InventoryItemBase first = mItemStack.Peek();

            if (first.Name == item.Name)
                return true;

            return false;
        }

        public bool Remove(InventoryItemBase item)
        {
            if (IsEmpty)
                return false;

            InventoryItemBase first = mItemStack.Peek();
            if (first.Name == item.Name)
            {
                mItemStack.Pop();
                return true;
            }

            return false;
        }
    }
}