using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Items
{
    public enum ItemType
    {
        None, Restorative, Field, Key, Weapon, Headgear, Shirt, Pants, Footwear, Accessory
    }

    public sealed class ItemTypeData : Enum<ItemType, ItemTypeData>
    {
        public static readonly ItemTypeData
            None = new ItemTypeData(nameof(None)),
            Restorative = new ItemTypeData(nameof(Restorative), "Rest"),
            Field = new ItemTypeData(nameof(Field)),
            Key = new ItemTypeData(nameof(Key)),
            Weapon = new ItemTypeData(nameof(Weapon)),
            Headgear = new ItemTypeData(nameof(Headgear), "Head"),
            Shirt = new ItemTypeData(nameof(Shirt)),
            Pants = new ItemTypeData(nameof(Pants)),
            Footwear = new ItemTypeData(nameof(Footwear), "Foot"),
            Accessory = new ItemTypeData(nameof(Accessory), "Acc");

        private ItemTypeData(string name) : this(name, name) { }

        private ItemTypeData(string name, string abbreviation)
        {
            Name = name;
            Abbreviation = abbreviation;
        }

        public string Name { get; }
        public string Abbreviation { get; }
    }

    public static class ItemTypeExtensions
    {
        public static ItemTypeData Get(this ItemType e) => ItemTypeData.GetData(e);
    }
}
