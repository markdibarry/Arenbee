using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class InventoryNull : IInventory
    {
        public ICollection<ItemStack> Items { get { return new List<ItemStack>().AsReadOnly(); } }
        public ItemStack GetItemStack(string itemId) { return null; }
        public ItemStack GetItemStack(Item item) { return null; }
        public ICollection<ItemStack> GetItemsByType(ItemType itemType) { return new List<ItemStack>(); }
        public int Add(Item item, int amount) { return 0; }
        public bool Remove(Item item, int amount) { return false; }
        public bool SetReservation(EquipmentSlot slot, Item newItem) { return false; }
        public bool RemoveReservation(EquipmentSlot slot, Item oldItem) { return false; }
    }
}
