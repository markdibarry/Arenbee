using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Items
{
    public enum EquipSlotName
    {
        None, Weapon, Headgear, Shirt, Pants, Footwear, Accessory1, Accessory2
    }

    public sealed class EquipSlotNameData : Enum<EquipSlotName, EquipSlotNameData>
    {
        public static readonly EquipSlotNameData
            None = new(nameof(None)),
            Weapon = new(nameof(Weapon)),
            Headgear = new(nameof(Headgear), "Head"),
            Shirt = new(nameof(Shirt)),
            Pants = new(nameof(Pants)),
            Footwear = new(nameof(Footwear), "Foot"),
            Accessory1 = new("Accessory 1", "Acc 1"),
            Accessory2 = new("Accessory 2", "Acc 2");

        private EquipSlotNameData(string name) : this(name, name) { }

        private EquipSlotNameData(string name, string abbreviation)
        {
            Name = name;
            Abbreviation = abbreviation;
        }

        public string Name { get; }
        public string Abbreviation { get; }
    }

    public static class EquipSlotNameExtensions
    {
        public static EquipSlotNameData Get(this EquipSlotName e) => EquipSlotNameData.GetData(e);
    }
}
