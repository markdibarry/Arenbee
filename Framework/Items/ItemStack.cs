using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arenbee.Framework.Items
{
    public class ItemStack
    {
        public ItemStack(string itemId, int amount)
        {
            Reservations = new List<EquipmentSlot>();
            ItemId = itemId;
            Amount = amount;
        }

        public string ItemId { get; private set; }
        public int Amount { get; private set; }
        [JsonIgnore]
        public ICollection<EquipmentSlot> Reservations { get; set; }

        public void AddAmount(int num)
        {
            Amount += num;
        }

        public void RemoveAmount(int num)
        {
            if (Amount > num)
                Amount -= num;
            else
                Amount = 0;
        }

        public bool AddReservation(EquipmentSlot slot)
        {
            if (!Reservations.Contains(slot) && Reservations.Count < Amount)
            {
                Reservations.Add(slot);
                return true;
            }
            return false;
        }

        public bool RemoveReservation(EquipmentSlot slot)
        {
            if (Reservations.Contains(slot))
            {
                Reservations.Remove(slot);
                return true;
            }
            return false;
        }
    }
}