using System.Text.Json.Serialization;

namespace Arenbee.Items;

public class EquipmentSlotData
{
    public EquipmentSlotData(EquipmentSlotData equipmentSlotData)
        : this(equipmentSlotData.SlotCategoryId, equipmentSlotData.ItemStackIndex)
    {
    }

    [JsonConstructor]
    public EquipmentSlotData(string slotCategoryId, int itemStackIndex)
    {
        ItemStackIndex = itemStackIndex;
        SlotCategoryId = slotCategoryId;
    }

    public int ItemStackIndex { get; } = -1;
    public string SlotCategoryId { get; }
}
