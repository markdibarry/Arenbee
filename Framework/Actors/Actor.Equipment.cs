using Arenbee.Framework.Statistics;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        private void InitEquipment()
        {
            SetInitialEquipmentStats();
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item oldItem, Item newItem)
        {
            oldItem?.ItemStats.RemoveFromStats(Stats);
            newItem?.ItemStats.AddToStats(Stats);
            Inventory.SetReservation(slot, newItem);
            UpdateEquipment();
        }

        private Stats GetStatsWithoutEquipment()
        {
            var newStats = new Stats(Stats);
            foreach (var slot in Equipment.Slots)
                slot.Item?.ItemStats?.RemoveFromStats(newStats);
            newStats.RecalculateStats();
            return newStats;
        }

        private void SetInitialEquipmentStats()
        {
            WeaponSlot.Init(this, Stats, StateController);
            foreach (var slot in Equipment.Slots)
            {
                if (slot.Item == null) continue;
                bool setSuccessful = Inventory.SetReservation(slot, slot.Item);
                if (!setSuccessful)
                    slot.SetItem(null);
                else
                    slot.Item?.ItemStats?.AddToStats(Stats);
            }
            UpdateEquipment();
        }

        private void UpdateEquipment()
        {
            var weapon = Equipment.GetSlot(EquipSlotName.Weapon).Item;
            WeaponSlot.SetWeapon(weapon);
            Stats.RecalculateStats();
        }
    }
}
