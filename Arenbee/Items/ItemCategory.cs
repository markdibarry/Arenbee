using GameCore.Items;

namespace Arenbee.Items;

public class ItemCategory : ItemCategoryBase
{
    public ItemCategory(string id, string name, string abbreviation)
        : base(id, name, abbreviation)
    {
    }
}

public static class ItemCategoryIds
{
    public const string Restorative = "Restorative";
    public const string Field = "Field";
    public const string Key = "Key";
    public const string Weapon = "Weapon";
    public const string Headgear = "Headgear";
    public const string Shirt = "Shirt";
    public const string Pants = "Pants";
    public const string Footwear = "Footwear";
    public const string Accessory = "Accessory";
}
