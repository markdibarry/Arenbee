using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class Inventory : IInventory
    {
        public Inventory()
        {
            _itemStacks = new List<ItemStack>();
        }

        public Inventory(ICollection<ItemStack> items)
        {
            _itemStacks = items.ToList();
        }

        public ICollection<ItemStack> Items { get { return _itemStacks.AsReadOnly(); } }
        private readonly List<ItemStack> _itemStacks;

        /// <summary>
        /// Returns the matching ItemStack for Item Id provided. Returns null if no stack is found.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>ItemStack</returns>
        public ItemStack GetItemStack(string itemId)
        {
            return _itemStacks.Find(itemSlot => itemSlot.ItemId.Equals(itemId));
        }

        /// <summary>
        /// Returns the matching ItemStack for item provided. Returns null if no stack is found.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ItemStack GetItemStack(Item item)
        {
            return _itemStacks.Find(itemStack => itemStack.ItemId.Equals(item?.Id));
        }

        public ICollection<ItemStack> GetItemsByType(ItemType itemType)
        {
            return _itemStacks.Where(itemStack => itemStack.Item.ItemType == itemType).ToList();
        }

        /// <summary>
        /// Adds a number of an item to the inventory.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        /// <returns>The number of items left over after attempting to add to the inventory.</returns>
        public int Add(Item item, int amount)
        {
            int leftOver;
            ItemStack itemStack = GetItemStack(item);
            if (itemStack == null)
            {
                if (amount > item.MaxStack)
                {
                    _itemStacks.Add(new ItemStack(item.Id, item.MaxStack));
                    leftOver = amount - item.MaxStack;
                }
                else
                {
                    _itemStacks.Add(new ItemStack(item.Id, amount));
                    leftOver = 0;
                }
            }
            else
            {
                if (itemStack.Amount + amount > item.MaxStack)
                {
                    int toAdd = item.MaxStack - itemStack.Amount;
                    itemStack.AddAmount(toAdd);
                    leftOver = amount - toAdd;
                }
                else
                {
                    itemStack.AddAmount(amount);
                    leftOver = 0;
                }
            }
            return leftOver;
        }

        public bool Remove(Item item, int amount)
        {
            ItemStack itemStack = GetItemStack(item);
            if (itemStack != null)
            {
                itemStack.RemoveAmount(amount);
                if (itemStack.Amount == 0)
                {
                    _itemStacks.Remove(itemStack);
                }
                return true;
            }
            return false;
        }

        public bool SetReservation(EquipmentSlot slot, Item newItem)
        {
            ItemStack oldStack = _itemStacks.Find(x => x.Reservations.Contains(slot));
            oldStack?.RemoveReservation(slot);
            ItemStack newStack = GetItemStack(newItem);
            if (newStack == null) return false;
            return newStack.AddReservation(slot);
        }

        public bool RemoveReservation(EquipmentSlot slot, Item oldItem)
        {
            ItemStack stack = GetItemStack(oldItem);
            if (stack == null) return false;
            return stack.RemoveReservation(slot);
        }
    }
}
