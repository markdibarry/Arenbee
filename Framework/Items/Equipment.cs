using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Enums;
using Newtonsoft.Json;

namespace Arenbee.Framework.Items
{
    public class Equipment
    {
        // TODO: Fix event subscription/ freeing
        public Equipment()
        {
            _slots = new List<EquipmentSlot>()
            {
                new EquipmentSlot(EquipmentSlotName.Weapon, ItemType.Weapon),
                new EquipmentSlot(EquipmentSlotName.Headgear, ItemType.Headgear),
                new EquipmentSlot(EquipmentSlotName.Shirt, ItemType.Shirt),
                new EquipmentSlot(EquipmentSlotName.Pants, ItemType.Pants),
                new EquipmentSlot(EquipmentSlotName.Footwear, ItemType.Footwear),
                new EquipmentSlot(EquipmentSlotName.Accessory1, ItemType.Accessory),
                new EquipmentSlot(EquipmentSlotName.Accessory2, ItemType.Accessory),
            };

            foreach (var slot in _slots)
            {
                slot.EquipmentSet += OnEquipmentSet;
                slot.EquipmentRemoved += OnEquipmentRemoved;
            }
        }

        public delegate void EquipmentSetHandler(EquipmentSlot slot, Item newItem);
        public delegate void EquipmentRemovedHandler(EquipmentSlot slot, Item oldItem);
        public event EquipmentSetHandler EquipmentSet;
        public event EquipmentRemovedHandler EquipmentRemoved;
        [JsonProperty]
        private readonly ICollection<EquipmentSlot> _slots;

        public ICollection<EquipmentSlot> GetAllSlots()
        {
            return _slots;
        }

        public ICollection<EquipmentSlot> GetSlotsByType(ItemType itemType)
        {
            return _slots.Where(x => x.SlotType == itemType).ToList();
        }

        public EquipmentSlot GetSlot(EquipmentSlotName slotName)
        {
            return _slots.First(x => x.SlotName.Equals(slotName));
        }

        public void OnEquipmentSet(EquipmentSlot slot, Item newItem)
        {
            EquipmentSet?.Invoke(slot, newItem);
        }

        public void OnEquipmentRemoved(EquipmentSlot slot, Item oldItem)
        {
            EquipmentRemoved?.Invoke(slot, oldItem);
        }
    }
}
