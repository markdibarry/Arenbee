using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Actors;
using GameCore.Utility;

namespace GameCore.Items;

public class EquipmentBase
{
    public EquipmentBase(Actor actor)
    {
        _slots = new();
        _actor = actor;
        foreach (var category in Locator.EquipSlotCategoryDB.Categories)
            _slots.Add(new EquipmentSlotBase(category.Id));
    }

    private readonly Actor _actor;
    private readonly List<EquipmentSlotBase> _slots;
    public IEnumerable<EquipmentSlotBase> Slots => _slots.AsReadOnly();
    public event Action<EquipmentSlotBase, ItemBase, ItemBase> EquipmentSet;

    /// <summary>
    /// Applies equipment if reservation available.
    /// </summary>
    /// <param name="slots"></param>
    public void ApplyEquipment(IEnumerable<EquipmentSlotBase> slots)
    {
        foreach (var slot in slots)
            TrySetItem(GetSlot(slot.SlotCategoryId), slot.Item);
    }

    public IEnumerable<EquipmentSlotBase> GetSlotsByType(string itemCategoryId)
    {
        return _slots.Where(x => x.SlotCategoryId == itemCategoryId).ToList();
    }

    public EquipmentSlotBase GetSlot(string slotCategoryId)
    {
        return _slots.First(x => x.SlotCategoryId.Equals(slotCategoryId));
    }

    public bool TrySetItemById(EquipmentSlotBase slot, string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return TrySetItem(slot, null);
        ItemBase item = Locator.ItemDB.GetItem(itemId);
        if (item == null)
            return false;
        return TrySetItem(slot, item);
    }

    public bool TrySetItem(EquipmentSlotBase slot, ItemBase newItem)
    {
        var inventory = _actor.Inventory;
        if (inventory == null)
            return false;
        if (newItem != null && !inventory.CanReserve(newItem))
            return false;
        if (!slot.IsCompatible(newItem))
            return false;
        inventory.SetReservation(slot, newItem);
        ItemBase oldItem = slot.Item;
        slot.Item = newItem;
        EquipmentSet?.Invoke(slot, oldItem, newItem);
        return true;
    }
}
