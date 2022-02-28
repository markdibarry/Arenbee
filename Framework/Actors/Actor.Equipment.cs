using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;
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
            Inventory.SetReservation(slot, newItem);
            UpdateEquipment();
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
            }
            UpdateEquipment();
        }

        private void UpdateEquipment()
        {
            var newStats = Equipment.GenerateStats(Stats);
            Stats.SetStats(newStats);
            var weapon = Equipment.GetSlot(EquipmentSlotName.Weapon).Item;
            WeaponSlot.SetWeapon(weapon);
        }
    }
}
