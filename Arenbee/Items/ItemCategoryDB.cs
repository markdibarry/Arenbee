using GameCore.Items;

namespace Arenbee.Items;

public class ItemCategoryDB : AItemCategoryDB
{
    protected override ItemCategory[] BuildDB()
    {
        return new ItemCategory[]
        {
            new ItemCategory(ItemCategoryIds.Restorative, "Restorative", "Rest"),
            new ItemCategory(ItemCategoryIds.Field, "Field", "Field"),
            new ItemCategory(ItemCategoryIds.Key, "Key", "Key"),
            new ItemCategory(ItemCategoryIds.Weapon, "Weapon", "Weap"),
            new ItemCategory(ItemCategoryIds.Headgear, "Headgear", "Head"),
            new ItemCategory(ItemCategoryIds.Shirt, "Shirt", "Shirt"),
            new ItemCategory(ItemCategoryIds.Pants, "Pants", "Pants"),
            new ItemCategory(ItemCategoryIds.Footwear, "Footwear", "Foot"),
            new ItemCategory(ItemCategoryIds.Accessory, "Accessory", "Acc")
        };
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
    public const string SubWeapon = "SubWeapon";
}
