using System.Collections.Generic;
using GameCore.Utility;
using System.Text.Json.Serialization;

namespace GameCore.Items;

public class ItemStack
{
    public ItemStack(string itemId, int amount)
    {
        Reservations = new List<EquipmentSlotBase>();
        ItemId = itemId;
        Amount = amount;
    }

    public int Amount { get; private set; }
    [JsonIgnore]
    public ItemBase Item
    {
        get => Locator.ItemDB.GetItem(ItemId);
        private set => ItemId = value?.Id;
    }
    public string ItemId { get; private set; }
    [JsonIgnore]
    public ICollection<EquipmentSlotBase> Reservations { get; set; }

    public void AddAmount(int num)
    {
        Amount += num;
    }

    public bool AddReservation(EquipmentSlotBase slot)
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

    public bool RemoveReservation(EquipmentSlotBase slot)
    {
        if (!Reservations.Contains(slot))
            return false;
        return Reservations.Remove(slot);
    }
}
