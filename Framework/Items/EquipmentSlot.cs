using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class EquipmentSlot
    {
        public EquipmentSlot(Actor actor, EquipmentSlotName slotName, ItemType slotType)
        {
            Actor = actor;
            SlotName = slotName;
            SlotType = slotType;
        }

        public Actor Actor { get; }
        public EquipmentSlotName SlotName { get; }
        public ItemType SlotType { get; }
        public string ItemId { get; private set; }
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
                Item oldItem = ItemDB.GetItem(ItemId);
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
