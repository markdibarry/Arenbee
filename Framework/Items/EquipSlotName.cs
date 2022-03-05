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
            None = new EquipSlotNameData(nameof(None)),
            Weapon = new EquipSlotNameData(nameof(Weapon)),
            Headgear = new EquipSlotNameData(nameof(Headgear), "Head"),
            Shirt = new EquipSlotNameData(nameof(Shirt)),
            Pants = new EquipSlotNameData(nameof(Pants)),
            Footwear = new EquipSlotNameData(nameof(Footwear), "Foot"),
            Accessory1 = new EquipSlotNameData("Accessory 1", "Acc 1"),
            Accessory2 = new EquipSlotNameData("Accessory 2", "Acc 2");

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
