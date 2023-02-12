using System.Collections.Generic;
using System.Linq;
using GameCore.Items;

namespace Arenbee.Items;

public class EquipmentSlotData
{
    public EquipmentSlotData(EquipmentSlot slot, IReadOnlyCollection<AItemStack> itemStacks)
    {
        SlotCategoryId = slot.SlotCategory.Id;
        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks.ElementAt(i) == slot.ItemStack)
            {
                ItemStackIndex = i;
                break;
            }
        }
    }

    public EquipmentSlotData(string slotCategoryId, int itemStackIndex)
    {
        ItemStackIndex = itemStackIndex;
        SlotCategoryId = slotCategoryId;
    }

    public int ItemStackIndex { get; } = -1;
    public string SlotCategoryId { get; }

    public EquipmentSlotData Clone() => new(SlotCategoryId, ItemStackIndex);
}
