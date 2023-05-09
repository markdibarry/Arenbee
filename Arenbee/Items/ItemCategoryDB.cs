using System.Collections.Generic;
using System.Linq;
using GameCore.Items;

namespace Arenbee.Items;

public class ItemCategoryDB : IItemCategoryDB
{
    public IReadOnlyCollection<ItemCategory> Categories { get; } = BuildDB();

    public ItemCategory? GetCategory(string id)
    {
        return Categories.FirstOrDefault(category => category.Id.Equals(id));
    }

    public IReadOnlyCollection<ItemCategory> GetCategories() => Categories;

    private static ItemCategory[] BuildDB()
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
