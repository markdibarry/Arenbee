using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Actor
    {
        private void InitEquipment()
        {
            if (_inventory == null)
                _inventory = new Inventory(this);

            if (_equipment == null)
                _equipment = new Equipment(this);
            else
                SetInitialEquipmentStats();

            _equipment.EquipmentRemoved += OnEquipmentRemoved;
            _equipment.EquipmentSet += OnEquipmentSet;
        }

        private void OnEquipmentRemoved(EquipmentSlot slot, Item oldItem)
        {
            _inventory.RemoveReservation(slot, oldItem);
            RemoveEquipmentStats(oldItem.ItemStats);
        }

        private void RemoveEquipmentStats(ItemStats itemStats)
        {
            if (itemStats != null)
            {
                foreach (ElementModifier mod in itemStats.DefenseElementModifiers.OrEmpty())
                {
                    if (DefenseElementModifiers.Contains(mod))
                        DefenseElementModifiers.Remove(mod);
                }

                foreach (StatusEffectModifier mod in itemStats.DefenseStatusEffects.OrEmpty())
                {
                    if (DefenseStatusEffects.Contains(mod))
                        DefenseStatusEffects.Remove(mod);
                }

                foreach (StatModifier mod in itemStats.StatModifiers.OrEmpty())
                {
                    var statMods = Stats[mod.StatType].StatModifiers;
                    if (statMods.Contains(mod))
                        statMods.Remove(mod);
                }
            }
            CalculateStats();
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item newItem)
        {
            _inventory.SetReservation(slot, newItem);
            SetEquipmentStats(newItem.ItemStats);
            if (newItem.ItemType == ItemType.Weapon)
            {
                WeaponSlot.AttachWeapon(newItem.Id);
            }
        }

        private void SetEquipmentStats(ItemStats itemStats)
        {
            if (itemStats != null)
            {
                foreach (ElementModifier mod in itemStats.DefenseElementModifiers.OrEmpty())
                {
                    if (!DefenseElementModifiers.Contains(mod))
                        DefenseElementModifiers.Add(mod);
                }

                foreach (StatusEffectModifier mod in itemStats.DefenseStatusEffects.OrEmpty())
                {
                    if (!DefenseStatusEffects.Contains(mod))
                        DefenseStatusEffects.Add(mod);
                }

                foreach (StatModifier mod in itemStats.StatModifiers.OrEmpty())
                {
                    var statMods = Stats[mod.StatType].StatModifiers;
                    if (!statMods.Contains(mod))
                        statMods.Add(mod);
                }
            }
            CalculateStats();
        }

        private void SetInitialEquipmentStats()
        {
            foreach (var slot in _equipment.GetAllSlots())
            {
                if (!string.IsNullOrEmpty(slot.ItemId))
                {
                    var item = ItemDB.GetItem(slot.ItemId);
                    if (item != null)
                        SetEquipmentStats(item.ItemStats);
                }
            }
        }
    }
}
