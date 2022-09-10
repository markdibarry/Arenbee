using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;

public class ItemCategoryDB : ItemCategoryDBBase
{
    protected override void BuildDB(List<ItemCategoryBase> categories)
    {
        categories.Add(new ItemCategory(ItemCategoryIds.Restorative, "Restorative", "Rest"));
        categories.Add(new ItemCategory(ItemCategoryIds.Field, "Field", "Field"));
        categories.Add(new ItemCategory(ItemCategoryIds.Key, "Key", "Key"));
        categories.Add(new ItemCategory(ItemCategoryIds.Weapon, "Weapon", "Weap"));
        categories.Add(new ItemCategory(ItemCategoryIds.Headgear, "Headgear", "Head"));
        categories.Add(new ItemCategory(ItemCategoryIds.Shirt, "Shirt", "Shirt"));
        categories.Add(new ItemCategory(ItemCategoryIds.Pants, "Pants", "Pants"));
        categories.Add(new ItemCategory(ItemCategoryIds.Footwear, "Footwear", "Foot"));
        categories.Add(new ItemCategory(ItemCategoryIds.Accessory, "Accessory", "Acc"));
    }
}
