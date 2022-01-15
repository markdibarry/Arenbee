using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class Equipment
    {
        public Equipment(Actor actor)
        {
            Actor = actor;
            Slots = new List<EquipmentSlot>
            {
                new EquipmentSlot() { SlotType = EquipmentType.Weapon },
                new EquipmentSlot() { SlotType = EquipmentType.Headgear },
                new EquipmentSlot() { SlotType = EquipmentType.Shirt },
                new EquipmentSlot() { SlotType = EquipmentType.Pants },
                new EquipmentSlot() { SlotType = EquipmentType.Footwear },
                new EquipmentSlot() { SlotType = EquipmentType.Accessory },
                new EquipmentSlot() { SlotType = EquipmentType.Accessory }
            };

            foreach (EquipmentSlot slot in Slots)
            {
                slot.RemovingItem += OnRemovingItem;
                slot.ItemSet += OnItemSet;
            }
        }

        public delegate void EquipmentChangedHandler(EquipableItem equipableItem);
        public event EquipmentChangedHandler RemovingEquipment;
        public event EquipmentChangedHandler EquipmentSet;
        public IEnumerable<EquipmentSlot> Slots { get; set; }
        public Actor Actor { get; set; }

        public bool CanEquip(EquipableItem item)
        {
            return false;
        }

        private void OnRemovingItem(EquipmentSlot slot)
        {
            EquipableItem item = slot.Item;
            Console.WriteLine(slot.SlotType.ToString() + " removed " + item.DisplayName);
            RemovingEquipment?.Invoke(item);
        }

        private void OnItemSet(EquipmentSlot slot)
        {
            EquipableItem item = slot.Item;
            Console.WriteLine(slot.SlotType.ToString() + " set to " + item.DisplayName);
            EquipmentSet?.Invoke(item);
        }
    }
}
