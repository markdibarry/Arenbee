using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class EquipmentSlot
    {
        public EquipmentSlot(EquipmentSlotName slotName, ItemType slotType)
        {
            SlotName = slotName;
            SlotType = slotType;
        }

        public EquipmentSlotName SlotName { get; }
        public ItemType SlotType { get; }
        public string ItemId { get; set; }
        public Item Item
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemId))
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = ItemDB.GetItem(ItemId);
                    return _item;
                }
                return null;
            }
        }
        private Item _item;
        public delegate void EquipmentSetHandler(EquipmentSlot slot, Item newItem);
        public delegate void EquipmentRemovedHandler(EquipmentSlot slot, Item oldItem);
        public event EquipmentSetHandler EquipmentSet;
        public event EquipmentRemovedHandler EquipmentRemoved;

        public void SetItem(Item newItem)
        {
            if (newItem != null && CanSetItem(newItem))
            {
                RemoveItem();
                ItemId = newItem.Id;
                EquipmentSet?.Invoke(this, newItem);
            }
        }

        public void RemoveItem()
        {
            if (!string.IsNullOrEmpty(ItemId))
            {
                Item oldItem = Item;
                ItemId = null;
                EquipmentRemoved?.Invoke(this, oldItem);
            }
        }

        private bool CanSetItem(Item item)
        {
            return item.ItemType == SlotType;
        }
    }
}
