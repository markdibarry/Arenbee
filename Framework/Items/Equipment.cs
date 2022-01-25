using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class Equipment
    {
        public Equipment(Actor actor)
        {
            _actor = actor;
            _slots = new List<EquipmentSlot>()
            {
                new EquipmentSlot(actor, EquipmentSlotName.Weapon, ItemType.Weapon),
                new EquipmentSlot(actor, EquipmentSlotName.Headgear, ItemType.Headgear),
                new EquipmentSlot(actor, EquipmentSlotName.Shirt, ItemType.Shirt),
                new EquipmentSlot(actor, EquipmentSlotName.Pants, ItemType.Pants),
                new EquipmentSlot(actor, EquipmentSlotName.Footwear, ItemType.Footwear),
                new EquipmentSlot(actor, EquipmentSlotName.Accessory1, ItemType.Accessory),
                new EquipmentSlot(actor, EquipmentSlotName.Accessory2, ItemType.Accessory),
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
        private readonly ICollection<EquipmentSlot> _slots;
        private readonly Actor _actor;

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
