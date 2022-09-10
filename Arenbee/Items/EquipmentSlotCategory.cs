using GameCore.Items;

namespace Arenbee.Items;

public class EquipmentSlotCategory : EquipmentSlotCategoryBase
{
    public EquipmentSlotCategory(string id, string name, string abbreviation, string itemCategoryId)
        : base(id, name, abbreviation, itemCategoryId)
    {
    }
}

public static class EquipmentSlotCategoryIds
{
    public const string Weapon = "Weapon";
    public const string Headgear = "Headgear";
    public const string Shirt = "Shirt";
    public const string Pants = "Pants";
    public const string Footwear = "Footwear";
    public const string Accessory1 = "Accessory1";
    public const string Accessory2 = "Accessory2";
}
