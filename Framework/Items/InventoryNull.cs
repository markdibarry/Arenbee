using System.Collections.Generic;

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
        public void SetReservation(EquipmentSlot slot, Item newItem) { }
    }
}
