using Godot;

namespace Arenbee.Statistics;

public static class StatTypeHelpers
{
    public static StatCategory GetStatCategory(int statType)
    {
        return GetStatCategory((StatType)statType);
    }

    public static StatCategory GetStatCategory(StatType statType)
    {
        return statType switch
        {
            StatType.Level or
            StatType.HP or
            StatType.MaxHP or
            StatType.MP or
            StatType.MaxMP or
            StatType.Attack or
            StatType.Defense or
            StatType.MagicAttack or
            StatType.MagicDefense or
            StatType.Luck or
            StatType.Evade or
            StatType.Speed => StatCategory.Attribute,
            StatType.AttackElement => StatCategory.AttackElement,
            StatType.FireResist or
            StatType.WaterResist or
            StatType.WindResist or
            StatType.EarthResist or
            StatType.PlantResist or
            StatType.DarkResist or
            StatType.LightResist or
            StatType.HealingResist => StatCategory.ElementResist,
            StatType.BurnResist or
            StatType.FreezeResist or
            StatType.ParalysisResist or
            StatType.PoisonResist or
            StatType.ZombieResist => StatCategory.StatusResist,
            StatType.KOAttack or
            StatType.BurnAttack or
            StatType.FreezeAttack or
            StatType.ParalysisAttack or
            StatType.PoisonAttack or
            StatType.ZombieAttack => StatCategory.StatusAttack,
            StatType.KO or
            StatType.Burn or
            StatType.Freeze or
            StatType.Paralysis or
            StatType.Poison or
            StatType.Zombie => StatCategory.StatusEffect,
            _ => StatCategory.None
        };
    }

    public static StatType GetStatType(AttributeType attributeType)
    {
        return attributeType switch
        {
            AttributeType.Level => StatType.Level,
            AttributeType.HP => StatType.HP,
            AttributeType.MaxHP => StatType.MaxHP,
            AttributeType.MP => StatType.MP,
            AttributeType.MaxMP => StatType.MaxMP,
            AttributeType.Attack => StatType.Attack,
            AttributeType.Defense => StatType.Defense,
            AttributeType.MagicAttack => StatType.MagicAttack,
            AttributeType.MagicDefense => StatType.MagicDefense,
            AttributeType.Luck => StatType.Luck,
            AttributeType.Evade => StatType.Evade,
            AttributeType.Speed => StatType.Speed,
            AttributeType.None or
            _ => StatType.None
        };
    }

    public static StatType GetStatusResistType(StatType statType)
    {
        return statType switch
        {
            StatType.KOAttack or
            StatType.KO => StatType.KOResist,
            StatType.BurnAttack or
            StatType.Burn => StatType.BurnResist,
            StatType.FreezeAttack or
            StatType.Freeze => StatType.FreezeResist,
            StatType.ParalysisAttack or
            StatType.Paralysis => StatType.ParalysisResist,
            StatType.PoisonAttack or
            StatType.Poison => StatType.PoisonResist,
            StatType.ZombieAttack or
            StatType.Zombie => StatType.ZombieResist,
            _ => StatType.None
        };
    }

    public static StatType GetStatusResistType(StatusEffectType statusEffectType)
    {
        return statusEffectType switch
        {
            StatusEffectType.KO => StatType.KOResist,
            StatusEffectType.Burn => StatType.BurnResist,
            StatusEffectType.Freeze => StatType.FreezeResist,
            StatusEffectType.Paralysis => StatType.ParalysisResist,
            StatusEffectType.Poison => StatType.PoisonResist,
            StatusEffectType.Zombie => StatType.ZombieResist,
            _ => StatType.None
        };
    }

    public static StatType GetStatusAttackType(StatType statType)
    {
        return statType switch
        {
            StatType.KOResist or
            StatType.KO => StatType.KOAttack,
            StatType.BurnResist or
            StatType.Burn => StatType.BurnAttack,
            StatType.FreezeResist or
            StatType.Freeze => StatType.FreezeAttack,
            StatType.ParalysisResist or
            StatType.Paralysis => StatType.ParalysisAttack,
            StatType.PoisonResist or
            StatType.Poison => StatType.PoisonAttack,
            StatType.ZombieResist or
            StatType.Zombie => StatType.ZombieAttack,
            _ => StatType.None
        };
    }

    public static StatType GetStatusAttackType(StatusEffectType statusEffectType)
    {
        return statusEffectType switch
        {
            StatusEffectType.KO => StatType.KOAttack,
            StatusEffectType.Burn => StatType.BurnAttack,
            StatusEffectType.Freeze => StatType.FreezeAttack,
            StatusEffectType.Paralysis => StatType.ParalysisAttack,
            StatusEffectType.Poison => StatType.PoisonAttack,
            StatusEffectType.Zombie => StatType.ZombieAttack,
            _ => StatType.None
        };
    }

    public static StatType GetStatusEffect(StatType statType)
    {
        return statType switch
        {
            StatType.KOAttack or
            StatType.KOResist => StatType.KO,
            StatType.BurnAttack or
            StatType.BurnResist => StatType.Burn,
            StatType.FreezeAttack or
            StatType.FreezeResist => StatType.Freeze,
            StatType.ParalysisAttack or
            StatType.ParalysisResist => StatType.Paralysis,
            StatType.PoisonAttack or
            StatType.PoisonResist => StatType.Poison,
            StatType.ZombieAttack or
            StatType.ZombieResist => StatType.Zombie,
            _ => StatType.None
        };
    }

    public static StatusEffectType GetStatusEffectType(StatType statType)
    {
        return statType switch
        {
            StatType.KO or
            StatType.KOAttack or
            StatType.KOResist => StatusEffectType.KO,
            StatType.Burn or
            StatType.BurnAttack or
            StatType.BurnResist => StatusEffectType.Burn,
            StatType.Freeze or
            StatType.FreezeAttack or
            StatType.FreezeResist => StatusEffectType.Freeze,
            StatType.Paralysis or
            StatType.ParalysisAttack or
            StatType.ParalysisResist => StatusEffectType.Paralysis,
            StatType.Poison or
            StatType.PoisonAttack or
            StatType.PoisonResist => StatusEffectType.Poison,
            StatType.Zombie or
            StatType.ZombieAttack or
            StatType.ZombieResist => StatusEffectType.Zombie,
            _ => StatusEffectType.None
        };
    }

    public static StatType GetStatusEffect(StatusEffectType statusEffectType)
    {
        return statusEffectType switch
        {
            StatusEffectType.KO => StatType.KO,
            StatusEffectType.Burn => StatType.Burn,
            StatusEffectType.Freeze => StatType.Freeze,
            StatusEffectType.Paralysis => StatType.Paralysis,
            StatusEffectType.Poison => StatType.Poison,
            StatusEffectType.Zombie => StatType.Zombie,
            _ => StatType.None
        };
    }

    public static StatType GetElementResist(ElementType elementType)
    {
        return elementType switch
        {
            ElementType.Fire => StatType.FireResist,
            ElementType.Wind => StatType.WindResist,
            ElementType.Water => StatType.WaterResist,
            ElementType.Earth => StatType.EarthResist,
            ElementType.Plant => StatType.PlantResist,
            ElementType.Dark => StatType.DarkResist,
            ElementType.Light => StatType.LightResist,
            ElementType.Healing => StatType.HealingResist,
            _ => StatType.None
        };
    }

    public static ElementType GetElement(StatType statType)
    {
        return statType switch
        {
            StatType.FireResist => ElementType.Fire,
            StatType.WindResist => ElementType.Wind,
            StatType.WaterResist => ElementType.Water,
            StatType.EarthResist => ElementType.Earth,
            StatType.PlantResist => ElementType.Plant,
            StatType.DarkResist => ElementType.Dark,
            StatType.LightResist => ElementType.Light,
            StatType.HealingResist => ElementType.Healing,
            _ => ElementType.None
        };
    }

    public static Color GetElementColor(ElementType elementType)
    {
        return elementType switch
        {
            ElementType.Fire => Colors.Red,
            ElementType.Wind => Colors.LightGreen,
            ElementType.Water => new Color(0, 0.5f, 1, 1),
            ElementType.Earth => new Color(0.65f, 0.45f, 0.15f, 1),
            ElementType.Plant => new Color(0.5f, 0.85f, 0, 1),
            ElementType.Dark => new Color(0.5f, 0.4f, 0.6f, 1),
            ElementType.Light => new Color(1, 1, 0.9f, 1),
            ElementType.Healing => Colors.Pink,
            _ => Colors.Gray
        };
    }
}
