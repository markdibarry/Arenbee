using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;

public class EquipmentSlotCategoryDB : AEquipmentSlotCategoryDB
{
    protected override EquipmentSlotCategory[] BuildDB()
    {
        return new EquipmentSlotCategory[]
        {
            new(EquipmentSlotCategoryIds.Weapon, ItemCategoryIds.Weapon, "Weapon", "Weap."),
            new(EquipmentSlotCategoryIds.Headgear, ItemCategoryIds.Headgear, "Headgear", "Head."),
            new(EquipmentSlotCategoryIds.Shirt, ItemCategoryIds.Shirt, "Shirt", "Shirt"),
            new(EquipmentSlotCategoryIds.Pants, ItemCategoryIds.Pants, "Pants", "Pants"),
            new(EquipmentSlotCategoryIds.Footwear, ItemCategoryIds.Footwear, "Footwear", "Foot."),
            new(EquipmentSlotCategoryIds.Accessory1, ItemCategoryIds.Accessory, "Accessory 1", "Acc.1"),
            new(EquipmentSlotCategoryIds.Accessory2, ItemCategoryIds.Accessory, "Accessory 2", "Acc.2"),
            new(EquipmentSlotCategoryIds.SubWeapon, ItemCategoryIds.SubWeapon, "Sub Weapon", "SubW.")
        };
    }

    protected override Dictionary<string, string[]> BuildPresetDB()
    {
        return new()
        {
            {
                EquipmentSlotPresetIds.BasicEquipment,
                new string[]
                {
                    EquipmentSlotCategoryIds.Weapon,
                    EquipmentSlotCategoryIds.Headgear,
                    EquipmentSlotCategoryIds.Shirt,
                    EquipmentSlotCategoryIds.Pants,
                    EquipmentSlotCategoryIds.Footwear,
                    EquipmentSlotCategoryIds.Accessory1,
                    EquipmentSlotCategoryIds.Accessory2,
                    EquipmentSlotCategoryIds.SubWeapon
                }
            }
        };
    }
}
