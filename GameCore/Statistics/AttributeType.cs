using GameCore.Utility;

namespace GameCore.Statistics
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
            Level = new(nameof(Level), "Lv", "Each increase in level is a milestone of your progress!"),
            HP = new(nameof(HP), "Health Points."),
            MaxHP = new("Max HP", "MaxHP", "The upper bounds on your Health Points."),
            MP = new(nameof(MP), "Magic Points."),
            MaxMP = new("Max MP", "MaxMP", "The upper bounds on your Magic Points."),
            Attack = new(nameof(Attack), "Att", "The base damage a character can deal."),
            Defense = new(nameof(Defense), "Def", "This makes getting hurt hurt less."),
            MagicAttack = new("Magic Attack", "M.Att", "Like Attack, but M A G I C."),
            MagicDefense = new("Magic Defense", "M.Def", "Magic Defense."),
            Luck = new(nameof(Luck), "Luck."),
            Evade = new(nameof(Evade), "Evd", "Evade."),
            Speed = new(nameof(Speed), "Spd", "How fast you move around, but not like Evade.");

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