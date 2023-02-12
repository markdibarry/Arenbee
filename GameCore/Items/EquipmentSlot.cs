using System;

namespace GameCore.Items;

public class EquipmentSlot
{
    public EquipmentSlot(EquipmentSlotCategory category)
    : this(category, null)
    {
    }

    public EquipmentSlot(EquipmentSlotCategory category, AItemStack? item)
    {
        SlotCategory = category;
        ItemStack = item;
    }

    public EquipmentSlotCategory SlotCategory { get; }
    public AItemStack? ItemStack { get; set; }
    public AItem? Item => ItemStack?.Item;

    public bool IsCompatible(AItem item)
    {
        return Array.IndexOf(SlotCategory.ItemCategoryIds, item.ItemCategory.Id) != -1;
    }
}
