using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Actors;

namespace GameCore.Items;

public class AEquipment
{
    public AEquipment(AInventory inventory, IEnumerable<EquipmentSlotCategory> categories)
    {
        _inventory = inventory;
        _slots = categories.Select(x => new EquipmentSlot(x)).ToArray();
    }

    public AEquipment(AInventory inventory, EquipmentSlot[] slots)
    {
        _inventory = inventory;
        _slots = slots;
    }

    private readonly AInventory _inventory;
    private readonly EquipmentSlot[] _slots;
    public IReadOnlyCollection<EquipmentSlot> Slots => _slots;
    public event Action<EquipmentSlot, AItem?, AItem?>? EquipmentSet;

    public IEnumerable<EquipmentSlot> GetSlotsByType(string itemCategoryId)
    {
        return _slots.Where(x => x.SlotCategory.ItemCategoryIds.Contains(itemCategoryId));
    }

    public EquipmentSlot? GetSlot(string slotCategoryId)
    {
        return _slots.FirstOrDefault(x => x.SlotCategory.Id.Equals(slotCategoryId));
    }

    public bool TrySetItem(AActor actor, EquipmentSlot slot, AItem item)
    {
        AItemStack? itemStack = _inventory.GetItemStacks(item)
            .FirstOrDefault(x => x.CanReserve());
        if (itemStack == null)
            return false;
        return TrySetItem(actor, slot, itemStack);
    }

    public bool TrySetItem(AActor actor, EquipmentSlot slot, AItemStack newItemStack)
    {
        if (!newItemStack.CanReserve())
            return false;
        if (!slot.IsCompatible(newItemStack.Item))
            return false;
        AItem? oldItem = slot.Item;
        slot.ItemStack?.RemoveReservation(slot);
        newItemStack.AddReservation(actor, slot);
        slot.ItemStack = newItemStack;
        EquipmentSet?.Invoke(slot, oldItem, newItemStack.Item);
        return true;
    }

    public void RemoveItem(EquipmentSlot slot)
    {
        if (slot.ItemStack == null)
            return;
        AItem? oldItem = slot.Item;
        slot.ItemStack.RemoveReservation(slot);
        slot.ItemStack = null;
        EquipmentSet?.Invoke(slot, oldItem, null);
    }
}
