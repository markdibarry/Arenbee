using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Arenbee.Framework.Items
{
    public class Inventory
    {
        public Inventory()
        {
            Items = new List<ItemStack>();
        }

        [JsonProperty]
        public ICollection<ItemStack> Items { get; set; }

        /// <summary>
        /// Returns the matching ItemStack for Item Id provided. Returns null if no stack is found.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ItemStack GetItemStack(string itemId)
        {
            return Items.FirstOrDefault(itemSlot => itemSlot.ItemId.Equals(itemId));
        }

        /// <summary>
        /// Returns the matching ItemStack for item provided. Returns null if no stack is found.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ItemStack GetItemStack(Item item)
        {
            return Items.FirstOrDefault(itemSlot => itemSlot.ItemId.Equals(item.Id));
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
                    Items.Add(new ItemStack(item.Id, item.MaxStack));
                    leftOver = amount - item.MaxStack;
                }
                else
                {
                    Items.Add(new ItemStack(item.Id, amount));
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
                    Items.Remove(itemStack);
                }
                return true;
            }
            return false;
        }

        public void SetReservation(EquipmentSlot slot, Item newItem)
        {
            ItemStack stack = GetItemStack(newItem);
            stack.AddReservation(slot);
        }

        public void RemoveReservation(EquipmentSlot slot, Item oldItem)
        {
            ItemStack stack = GetItemStack(oldItem);
            stack.RemoveReservation(slot);
        }
    }
}
