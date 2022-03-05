using System.Collections.Generic;
using System.Linq;

namespace Arenbee.Framework.Items
{
    public class Equipment
    {
        // TODO: Fix event subscription/ freeing
        public Equipment()
        {
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

            SubscribeEvents();
        }

        public Equipment(IEnumerable<EquipmentSlot> slots)
        {
            _slots = slots.ToList();
            SubscribeEvents();
        }

        public IEnumerable<EquipmentSlot> Slots
        {
            get { return _slots.AsReadOnly(); }
        }

        private readonly List<EquipmentSlot> _slots;
        public delegate void EquipmentSetHandler(EquipmentSlot slot, Item oldItem, Item newItem);
        public event EquipmentSetHandler EquipmentSet;

        public IEnumerable<EquipmentSlot> GetSlotsByType(ItemType itemType)
        {
            return _slots.Where(x => x.SlotType == itemType).ToList();
        }

        public EquipmentSlot GetSlot(EquipSlotName slotName)
        {
            return _slots.First(x => x.SlotName.Equals(slotName));
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item oldItem, Item newItem)
        {
            EquipmentSet?.Invoke(slot, oldItem, newItem);
        }

        private void SubscribeEvents()
        {
            foreach (var slot in _slots)
                slot.EquipmentSet += OnEquipmentSet;
        }
    }
}
