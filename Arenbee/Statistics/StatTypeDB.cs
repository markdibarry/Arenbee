using System.Collections.Generic;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public class StatTypeDB : AStatTypeDB
{
    public static StatTypeData GetStatCategoryData(StatCategory statCategory)
    {
        if (s_statCategories.TryGetValue(statCategory, out StatTypeData? result))
            return result;
        GD.PrintErr($"StatCategory {statCategory} not defined.");
        return s_statCategories[StatCategory.None];
    }

    public static StatTypeData GetStatTypeData(AttributeType attributeType)
    {
        return GetStatTypeData(StatTypeHelpers.GetStatType(attributeType));
    }

    public static StatTypeData GetStatTypeData(StatType statType)
    {
        if (s_statTypes.TryGetValue(statType, out StatTypeData? result))
            return result;
        GD.PrintErr($"StatType {statType} not defined.");
        return s_statTypes[StatType.None];
    }

    public override string[] GetTypeNames()
    {
        return System.Enum.GetNames(typeof(StatType));
    }

    public override string[]? GetValueEnumOptions(int statType)
    {
        StatCategory category = StatTypeHelpers.GetStatCategory(statType);
        if (category == StatCategory.AttackElement)
            return System.Enum.GetNames(typeof(ElementType));
        if (category == StatCategory.ElementResist)
            return ElementResist.GetGodotEnum();
        return null;
    }

    private static readonly Dictionary<StatType, StatTypeData> s_statTypes = new()
    {
        {
            StatType.None,
            new()
            {
                Name = "None",
                Abbreviation = "None",
                Description = "None"
            }
        },
        {
            StatType.Level,
            new()
            {
                Name = "Level",
                Abbreviation = "Lv",
                Description = "Each increase in level is a milestone of your progress!"
            }
        },
        {
            StatType.HP,
            new()
            {
                Name = "HP",
                Abbreviation = "HP",
                Description = "Health Points."
            }
        },
        {
            StatType.MaxHP,
            new()
            {
                Name = "MaxHP",
                Abbreviation = "MaxHP",
                Description = "The upper bounds on your Health Points."
            }
        },
        {
            StatType.MP,
            new()
            {
                Name = "MP",
                Abbreviation = "MP",
                Description = "Magic Points"
            }
        },
        {
            StatType.MaxMP,
            new()
            {
                Name = "MaxMP",
                Abbreviation = "MaxMP",
                Description = "The upper bounds on your Magic Points."
            }
        },
        {
            StatType.Attack,
            new()
            {
                Name = "Attack",
                Abbreviation = "Atk",
                Description = "The base damage a character can deal."
            }
        },
        {
            StatType.Defense,
            new()
            {
                Name = "Defense",
                Abbreviation = "Def",
                Description = "This makes getting hurt hurt less."
            }
        },
        {
            StatType.MagicAttack,
            new()
            {
                Name = "MagicAttack",
                Abbreviation = "M.Atk",
                Description = "Like Attack, but M A G I C."
            }
        },
        {
            StatType.MagicDefense,
            new()
            {
                Name = "MagicDefense",
                Abbreviation = "M.Def",
                Description = "Magic Defense."
            }
        },
        {
            StatType.Luck,
            new()
            {
                Name = "Luck",
                Abbreviation = "Lck",
                Description = "Luck."
            }
        },
        {
            StatType.Evade,
            new()
            {
                Name = "Evade",
                Abbreviation = "Evd",
                Description = "Evasion."
            }
        },
        {
            StatType.Speed,
            new()
            {
                Name = "Speed",
                Abbreviation = "Spd",
                Description = "How fast you move around, but not like Evade."
            }
        },
    };

    private static readonly Dictionary<StatCategory, StatTypeData> s_statCategories = new()
    {
        {
            StatCategory.None,
            new()
            {
                Name = "None",
                Abbreviation = "None",
                Description = "None"
            }
        },
        {
            StatCategory.Attribute,
            new()
            {
                Name = "Attribute",
                Abbreviation = "Attr",
                Description = "Main stats"
            }
        },
        {
            StatCategory.AttackElement,
            new()
            {
                Name = "Attack Element",
                Abbreviation = "E.Atk",
                Description = "The element physical attacks are imbued with."
            }
        },
        {
            StatCategory.ElementResist,
            new()
            {
                Name = "Element Resistance",
                Abbreviation = "E.Def",
                Description = "Elemental resistences and weaknesses."
            }
        },
        {
            StatCategory.StatusAttack,
            new()
            {
                Name = "Status Attacks",
                Abbreviation = "S.Atk",
                Description = "Chance for physical attacks to cause status effects."
            }
        },
        {
            StatCategory.StatusResist,
            new()
            {
                Name = "Status Resistance",
                Abbreviation = "S.Def",
                Description = "Chance to resist status effects."
            }
        },
        {
            StatCategory.StatusEffect,
            new()
            {
                Name = "Status Effect",
                Abbreviation = "S.Eff",
                Description = "Status effects... I dunno."
            }
        },
    };
}
