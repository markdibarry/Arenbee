using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Statistics;

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

        public Equipment CloneEquipment()
        {
            var result = new List<EquipmentSlot>();
            foreach (var slot in _slots)
            {
                result.Add(new EquipmentSlot(slot.SlotName, slot.SlotType)
                {
                    ItemId = slot.ItemId
                });
            }
            return new Equipment(result);
        }

        public IEnumerable<EquipmentSlot> GetSlotsByType(ItemType itemType)
        {
            return _slots.Where(x => x.SlotType == itemType).ToList();
        }

        public EquipmentSlot GetSlot(EquipmentSlotName slotName)
        {
            return _slots.First(x => x.SlotName.Equals(slotName));
        }

        public Stats GenerateStats(Stats oldStats)
        {
            var newStats = new Stats { Attributes = oldStats.CloneBaseAttributes() };
            foreach (var slot in _slots)
            {
                var itemStats = slot.Item?.ItemStats;
                if (itemStats == null) continue;

                foreach (ElementModifier mod in itemStats.DefenseElementModifiers)
                {
                    newStats.DefenseElementModifiers.Add(mod);
                }

                foreach (StatusEffectModifier mod in itemStats.DefenseStatusEffects)
                {
                    newStats.DefenseStatusEffects.Add(mod);
                }

                foreach (AttributeModifier mod in itemStats.AttributeModifiers)
                {
                    newStats.Attributes[mod.AttributeType].AttributeModifiers.Add(mod);
                }
            }
            newStats.UpdateStats();
            return newStats;
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item oldItem, Item newItem)
        {
            EquipmentSet?.Invoke(slot, oldItem, newItem);
        }

        private void SubscribeEvents()
        {
            foreach (var slot in _slots)
            {
                slot.EquipmentSet += OnEquipmentSet;
            }
        }
    }
}
