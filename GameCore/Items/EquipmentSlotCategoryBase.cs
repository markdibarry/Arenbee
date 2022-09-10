using GameCore.Utility;

namespace GameCore.Items;

public class EquipmentSlotCategoryBase
{
    public EquipmentSlotCategoryBase(string id, string name, string abbreviation, string itemCategoryId)
    {
        Id = id;
        Name = name;
        Abbreviation = abbreviation;
        ItemCategoryId = itemCategoryId;
    }

    public string Id { get; }
    public string Name { get; }
    public string Abbreviation { get; }
    public ItemCategoryBase ItemCategory => Locator.ItemCategoryDB.GetType(ItemCategoryId);
    public string ItemCategoryId { get; }
}
