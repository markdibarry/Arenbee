using System.Collections.Generic;
using System.Linq;
using GameCore.Actors;
using GameCore.Utility;

namespace GameCore.Items
{
    public class Equipment
    {
        public Equipment(Actor actor)
        {
            _itemDB = Locator.ItemDB;
            _actor = actor;
            _slots = new List<EquipmentSlot>()
            {
                new EquipmentSlot(EquipSlotName.Weapon, ItemType.Weapon),
                new EquipmentSlot(EquipSlotName.Headgear, ItemType.Headgear),
                new EquipmentSlot(EquipSlotName.Shirt, ItemType.Shirt),
                new EquipmentSlot(EquipSlotName.Pants, ItemType.Pants),
                new EquipmentSlot(EquipSlotName.Footwear, ItemType.Footwear),
                new EquipmentSlot(EquipSlotName.Accessory1, ItemType.Accessory),
                new EquipmentSlot(EquipSlotName.Accessory2, ItemType.Accessory),
            };
        }

        private readonly Actor _actor;
        private readonly ItemDBBase _itemDB;
        private readonly List<EquipmentSlot> _slots;
        public IEnumerable<EquipmentSlot> Slots
        {
            get { return _slots.AsReadOnly(); }
        }
        public delegate void EquipmentSetHandler(EquipmentSlot slot, Item oldItem, Item newItem);
        public event EquipmentSetHandler EquipmentSet;

        /// <summary>
        /// Applies equipment if reservation available.
        /// </summary>
        /// <param name="slots"></param>
        public void ApplyEquipment(IEnumerable<EquipmentSlot> slots)
        {
            foreach (var slot in slots)
                TrySetItem(GetSlot(slot.SlotName), slot.Item);
        }

        public IEnumerable<EquipmentSlot> GetSlotsByType(ItemType itemType)
        {
            return _slots.Where(x => x.SlotType == itemType).ToList();
        }

        public EquipmentSlot GetSlot(EquipSlotName slotName)
        {
            return _slots.First(x => x.SlotName.Equals(slotName));
        }

        public bool TrySetItemById(EquipmentSlot slot, string itemId)
        {
            if (itemId == null)
                return TrySetItem(slot, null);
            Item item = _itemDB.GetItem(itemId);
            if (item == null) return false;
            return TrySetItem(slot, item);
        }

        public bool TrySetItem(EquipmentSlot slot, Item newItem)
        {
            var inventory = _actor.Inventory;
            if (inventory == null)
                return false;
            if (newItem != null && !inventory.CanReserve(newItem))
                return false;
            if (!slot.IsCompatible(newItem))
                return false;
            inventory.SetReservation(slot, newItem);
            Item oldItem = slot.Item;
            slot.SetItem(newItem);
            EquipmentSet?.Invoke(slot, oldItem, newItem);
            return true;
        }
    }
}
