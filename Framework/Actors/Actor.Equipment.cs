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
            if (Inventory == null)
                Inventory = new Inventory();

            if (Equipment == null)
                Equipment = new Equipment();
            else
                SetInitialEquipmentStats();

            CalculateStats();
        }

        private void OnEquipmentRemoved(EquipmentSlot slot, Item oldItem)
        {
            Inventory.RemoveReservation(slot, oldItem);
            RemoveEquipmentStats(oldItem.ItemStats);
            CalculateStats();
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
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item newItem)
        {
            Inventory.SetReservation(slot, newItem);
            SetEquipmentStats(newItem.ItemStats);
            CalculateStats();
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
        }

        private void SetInitialEquipmentStats()
        {
            foreach (var slot in Equipment.Slots)
            {
                if (!string.IsNullOrEmpty(slot.ItemId))
                {
                    var item = slot.Item;
                    if (item != null)
                    {
                        ItemStack itemStack = Inventory.GetItemStack(item);
                        if (itemStack != null)
                        {
                            Inventory.SetReservation(slot, item);
                            SetEquipmentStats(item.ItemStats);
                        }
                        else
                        {
                            slot.RemoveItem();
                        }
                    }
                }
            }
        }
    }
}
