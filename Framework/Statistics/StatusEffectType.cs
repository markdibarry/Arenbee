using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Statistics
{
    public enum StatusEffectType
    {
        None,
        Burn,
        Freeze,
        Paralysis,
        Poison,
        Zombie,
        Attack
    }

    public sealed class StatusEffectTypeData : Enum<StatusEffectType, StatusEffectTypeData>
    {
        public static readonly StatusEffectTypeData
            None = new(nameof(None), "None", "None", "None"),
            Burn = new(nameof(Burn), "Burn", "Burned", "Character is burned"),
            Freeze = new(nameof(Freeze), "Frz", "Frozen", "Character cannot move"),
            Paralysis = new(nameof(Paralysis), "Pyz", "Paralyzed", "Character cannot move."),
            Poison = new(nameof(Poison), "Psn", "Poisoned", "Slowly drains your life."),
            Zombie = new(nameof(Zombie), "Zom", "Zombified", "Character takes damage from healing."),
            Attack = new(nameof(Attack), "Atk", "Attack", "Attack is modified.");

        private StatusEffectTypeData(string name, string abbreviation, string pastTense, string description)
        {
            Name = name;
            Abbreviation = abbreviation;
            Description = description;
            PastTense = pastTense;
        }

        public string Name { get; }
        public string Abbreviation { get; }
        public string Description { get; }
        public string PastTense { get; }
    }

    public static class StatusEffectTypeExtensions
    {
        public static StatusEffectTypeData Get(this StatusEffectType e) => StatusEffectTypeData.GetData(e);
    }
}