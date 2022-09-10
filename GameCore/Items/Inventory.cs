using System.Collections.Generic;
using System.Linq;

namespace GameCore.Items;

public class Inventory
{
    public Inventory()
    {
        _itemStacks = new List<ItemStack>();
    }

    public Inventory(ICollection<ItemStack> items)
    {
        _itemStacks = items.ToList();
    }

    private readonly List<ItemStack> _itemStacks;
    public ICollection<ItemStack> Items => _itemStacks.AsReadOnly();

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
    public ItemStack GetItemStack(ItemBase item)
    {
        return _itemStacks.Find(itemStack => itemStack.ItemId.Equals(item?.Id));
    }

    public ICollection<ItemStack> GetItemsByType(string itemCategoryId)
    {
        return _itemStacks.Where(itemStack => itemStack.Item.ItemCategoryId == itemCategoryId).ToList();
    }

    /// <summary>
    /// Adds a number of an item to the inventory.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="amount"></param>
    /// <returns>The number of items left over after attempting to add to the inventory.</returns>
    public int Add(ItemBase item, int amount)
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

    public bool CanReserve(ItemBase item) => CanReserve(item?.Id);

    public bool CanReserve(string itemId)
    {
        return GetItemStack(itemId)?.CanReserve() ?? false;
    }

    public bool Remove(ItemBase item, int amount)
    {
        ItemStack itemStack = GetItemStack(item);
        if (itemStack != null)
        {
            itemStack.RemoveAmount(amount);
            if (itemStack.Amount == 0)
                _itemStacks.Remove(itemStack);
            return true;
        }
        return false;
    }

    public void SetReservation(EquipmentSlotBase slot, ItemBase newItem)
    {
        GetItemStack(slot.Item)?.RemoveReservation(slot);
        GetItemStack(newItem)?.AddReservation(slot);
    }
}
