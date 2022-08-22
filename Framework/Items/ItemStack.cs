using System.Collections.Generic;
using Arenbee.Framework.Utility;
using System.Text.Json.Serialization;

namespace Arenbee.Framework.Items
{
    public class ItemStack
    {
        public ItemStack(string itemId, int amount)
        {
            Reservations = new List<EquipmentSlot>();
            ItemId = itemId;
            Amount = amount;
            _itemDB = Locator.GetItemDB();
        }

        private readonly IItemDB _itemDB;
        private Item _item;
        public int Amount { get; private set; }
        [JsonIgnore]
        public Item Item
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemId))
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = _itemDB.GetItem(ItemId);
                    return _item;
                }
                return null;
            }
        }
        public string ItemId { get; }
        [JsonIgnore]
        public ICollection<EquipmentSlot> Reservations { get; set; }

        public void AddAmount(int num)
        {
            Amount += num;
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

        public bool RemoveReservation(EquipmentSlot slot)
        {
            if (!Reservations.Contains(slot))
                return false;
            return Reservations.Remove(slot);
        }
    }
}
