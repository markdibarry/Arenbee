using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;

public class EquipmentSlotCategoryDB : AEquipmentSlotCategoryDB
{
    protected override EquipmentSlotCategory[] BuildDB()
    {
        return new EquipmentSlotCategory[]
        {
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Weapon, ItemCategoryIds.Weapon, "Weapon", "Weap."),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Headgear, ItemCategoryIds.Headgear, "Headgear", "Head."),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Shirt, ItemCategoryIds.Shirt, "Shirt", "Shirt"),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Pants, ItemCategoryIds.Pants, "Pants", "Pants"),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Footwear, ItemCategoryIds.Footwear, "Footwear", "Foot."),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Accessory1, ItemCategoryIds.Accessory, "Accessory 1", "Acc.1"),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.Accessory2, ItemCategoryIds.Accessory, "Accessory 2", "Acc.2"),
            new EquipmentSlotCategory(EquipmentSlotCategoryIds.SubWeapon, ItemCategoryIds.SubWeapon, "Sub Weapon", "SubW.")
        };
    }

    public IReadOnlyCollection<EquipmentSlotCategory> BasicEquipment => Categories;
}

public static class EquipmentSlotCategoryIds
{
    public const string SubWeapon = "SubWeapon";
    public const string Weapon = "Weapon";
    public const string Headgear = "Headgear";
    public const string Shirt = "Shirt";
    public const string Pants = "Pants";
    public const string Footwear = "Footwear";
    public const string Accessory1 = "Accessory1";
    public const string Accessory2 = "Accessory2";
}
