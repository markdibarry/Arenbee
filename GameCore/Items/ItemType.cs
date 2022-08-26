using GameCore.Utility;

namespace GameCore.Items
{
    public enum ItemType
    {
        None, Restorative, Field, Key, Weapon, Headgear, Shirt, Pants, Footwear, Accessory
    }

    public sealed class ItemTypeData : Enum<ItemType, ItemTypeData>
    {
        public static readonly ItemTypeData
            None = new(nameof(None)),
            Restorative = new(nameof(Restorative), "Rest"),
            Field = new(nameof(Field)),
            Key = new(nameof(Key)),
            Weapon = new(nameof(Weapon)),
            Headgear = new(nameof(Headgear), "Head"),
            Shirt = new(nameof(Shirt)),
            Pants = new(nameof(Pants)),
            Footwear = new(nameof(Footwear), "Foot"),
            Accessory = new(nameof(Accessory), "Acc");

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
