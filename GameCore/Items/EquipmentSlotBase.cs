using System.Text.Json.Serialization;
using GameCore.Utility;

namespace GameCore.Items;

public class EquipmentSlotBase
{
    [JsonConstructor]
    public EquipmentSlotBase(string slotCategoryId, string itemId)
    {
        SlotCategoryId = slotCategoryId;
        ItemId = itemId;
    }

    public EquipmentSlotBase(string slotCategoryId)
        : this(slotCategoryId, null)
    { }

    public EquipmentSlotBase(EquipmentSlotBase slot)
        : this(slot.SlotCategoryId, slot.ItemId)
    { }

    [JsonIgnore]
    public ItemBase Item
    {
        get => Locator.ItemDB.GetItem(ItemId);
        set => ItemId = value?.Id;
    }
    public string ItemId { get; private set; }
    public string SlotCategoryId { get; }
    public EquipmentSlotCategoryBase SlotCategory => Locator.EquipSlotCategoryDB.GetCategory(SlotCategoryId);

    public bool IsCompatible(ItemBase item) => item == null || item.ItemCategoryId == SlotCategory.ItemCategoryId;
}
