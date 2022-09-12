using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;

public class EquipmentSlotCategoryDB : EquipmentSlotCategoryDBBase
{
    protected override void BuildDB(List<EquipmentSlotCategoryBase> categories)
    {
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Weapon, "Weapon", "Weap", ItemCategoryIds.Weapon));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Headgear, "Headgear", "Head", ItemCategoryIds.Headgear));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Shirt, "Shirt", "Shirt", ItemCategoryIds.Shirt));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Pants, "Pants", "Pants", ItemCategoryIds.Pants));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Footwear, "Footwear", "Foot", ItemCategoryIds.Footwear));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Accessory1, "Accessory 1", "Acc1", ItemCategoryIds.Accessory));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.Accessory2, "Accessory 2", "Acc2", ItemCategoryIds.Accessory));
        categories.Add(new EquipmentSlotCategory(EquipmentSlotCategoryIds.SubWeapon, "SubWeapon", "SubW", ItemCategoryIds.SubWeapon));
    }
}
