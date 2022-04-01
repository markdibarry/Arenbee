using System.Collections.Generic;

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
        void SetReservation(EquipmentSlot slot, Item newItem);
    }
}
