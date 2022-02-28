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

        public string ItemId { get; }
        [JsonIgnore]
        public Item Item
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemId))
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = ItemDB.GetItem(ItemId);
                    return _item;
                }
                return null;
            }
        }
        public int Amount { get; private set; }
        [JsonIgnore]
        public ICollection<EquipmentSlot> Reservations { get; set; }
        private Item _item;

        public void AddAmount(int num)
        {
            Amount += num;
        }

        public bool CanReserve()
        {
            return Reservations.Count < Amount;
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
            if (!Reservations.Contains(slot) && CanReserve())
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