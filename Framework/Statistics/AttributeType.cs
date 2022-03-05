using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Statistics
{
    public enum AttributeType
    {
        Level,
        HP,
        MaxHP,
        MP,
        MaxMP,
        Attack,
        Defense,
        MagicAttack,
        MagicDefense,
        Luck,
        Evade,
        Speed
    }

    public sealed class AttributeTypeData : Enum<AttributeType, AttributeTypeData>
    {
        public static readonly AttributeTypeData
            Level = new AttributeTypeData(nameof(Level), "Lv",
                "Each increase in level is a milestone of your progress!"),
            HP = new AttributeTypeData(nameof(HP),
                "Health Points."),
            MaxHP = new AttributeTypeData("Max HP", "MaxHP",
                "The upper bounds on your Health Points."),
            MP = new AttributeTypeData(nameof(MP),
                "Magic Points."),
            MaxMP = new AttributeTypeData("Max MP", "MaxMP",
                "The upper bounds on your Magic Points."),
            Attack = new AttributeTypeData(nameof(Attack), "Att",
                "The base damage a character can deal."),
            Defense = new AttributeTypeData(nameof(Defense), "Def",
                "This makes getting hurt hurt less."),
            MagicAttack = new AttributeTypeData("Magic Attack", "M.Att",
                "Like Attack, but M A G I C."),
            MagicDefense = new AttributeTypeData("Magic Defense", "M.Def",
                "Magic Defense."),
            Luck = new AttributeTypeData(nameof(Luck),
                "Luck."),
            Evade = new AttributeTypeData(nameof(Evade), "Evd",
                "Evade."),
            Speed = new AttributeTypeData(nameof(Speed), "Spd",
                "How fast you move around, but not like Evade.");

        private AttributeTypeData(string name, string description)
            : this(name, name, description)
        {
        }

        private AttributeTypeData(string name, string abbreviation, string description)
        {
            Name = name;
            Abbreviation = abbreviation;
            Description = description;
        }

        public string Name { get; }
        public string Abbreviation { get; }
        public string Description { get; }
    }

    public static class AttributeTypeExtensions
    {
        public static AttributeTypeData Get(this AttributeType e) => AttributeTypeData.GetData(e);
    }
}