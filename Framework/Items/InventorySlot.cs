using System;

namespace Arenbee.Framework.Items
{
    public class InventorySlot
    {
        public event EventHandler ItemSet;
        public event EventHandler RemovingItem;
        private Item _item;

        public Item GetItem()
        {
            return _item;
        }

        public void SetItem(Item item)
        {
            RemovingItem?.Invoke(this, EventArgs.Empty);
            _item = null;
            _item = item;
            ItemSet?.Invoke(this, EventArgs.Empty);
        }
    }
}