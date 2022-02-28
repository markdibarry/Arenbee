using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public interface IInventory
    {
        ICollection<ItemStack> Items { get; }
        ItemStack GetItemStack(string itemId);
        ItemStack GetItemStack(Item item);
        ICollection<ItemStack> GetItemsByType(ItemType itemType);
        int Add(Item item, int amount);
        bool Remove(Item item, int amount);
        bool RemoveReservation(EquipmentSlot slot, Item oldItem);
        bool SetReservation(EquipmentSlot slot, Item newItem);
    }
}
