using System.Collections.Generic;
using Arenbee.GUI.Localization;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public class StatTypeDB : BaseStatTypeDB
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
                Name = StatTypes.StatTypes_None_Name,
                Abbreviation = StatTypes.StatTypes_None_Abbreviation,
                Description = StatTypes.StatTypes_None_Description
            }
        },
        {
            StatType.Level,
            new()
            {
                Name = StatTypes.StatTypes_Level_Name,
                Abbreviation = StatTypes.StatTypes_Level_Abbreviation,
                Description = StatTypes.StatTypes_Level_Description
            }
        },
        {
            StatType.HP,
            new()
            {
                Name = StatTypes.StatTypes_HP_Name,
                Abbreviation = StatTypes.StatTypes_HP_Abbreviation,
                Description = StatTypes.StatTypes_HP_Description
            }
        },
        {
            StatType.MaxHP,
            new()
            {
                Name = StatTypes.StatTypes_MaxHP_Name,
                Abbreviation = StatTypes.StatTypes_MaxHP_Abbreviation,
                Description = StatTypes.StatTypes_MaxHP_Description
            }
        },
        {
            StatType.MP,
            new()
            {
                Name = StatTypes.StatTypes_MP_Name,
                Abbreviation = StatTypes.StatTypes_MP_Abbreviation,
                Description = StatTypes.StatTypes_MP_Description
            }
        },
        {
            StatType.MaxMP,
            new()
            {
                Name = StatTypes.StatTypes_MaxMP_Name,
                Abbreviation = StatTypes.StatTypes_MaxMP_Abbreviation,
                Description = StatTypes.StatTypes_MaxMP_Description
            }
        },
        {
            StatType.Attack,
            new()
            {
                Name = StatTypes.StatTypes_Attack_Name,
                Abbreviation = StatTypes.StatTypes_Attack_Abbreviation,
                Description = StatTypes.StatTypes_Attack_Description
            }
        },
        {
            StatType.Defense,
            new()
            {
                Name = StatTypes.StatTypes_Defense_Name,
                Abbreviation = StatTypes.StatTypes_Defense_Abbreviation,
                Description = StatTypes.StatTypes_Defense_Description
            }
        },
        {
            StatType.MagicAttack,
            new()
            {
                Name = StatTypes.StatTypes_MagicAttack_Name,
                Abbreviation = StatTypes.StatTypes_MagicAttack_Abbreviation,
                Description = StatTypes.StatTypes_MagicAttack_Description
            }
        },
        {
            StatType.MagicDefense,
            new()
            {
                Name = StatTypes.StatTypes_MagicDefense_Name,
                Abbreviation = StatTypes.StatTypes_MagicDefense_Abbreviation,
                Description = StatTypes.StatTypes_MagicDefense_Description
            }
        },
        {
            StatType.Luck,
            new()
            {
                Name = StatTypes.StatTypes_Luck_Name,
                Abbreviation = StatTypes.StatTypes_Luck_Abbreviation,
                Description = StatTypes.StatTypes_Luck_Description
            }
        },
        {
            StatType.Evade,
            new()
            {
                Name = StatTypes.StatTypes_Evade_Name,
                Abbreviation = StatTypes.StatTypes_Evade_Abbreviation,
                Description = StatTypes.StatTypes_Evade_Description
            }
        },
        {
            StatType.Speed,
            new()
            {
                Name = StatTypes.StatTypes_Speed_Name,
                Abbreviation = StatTypes.StatTypes_Speed_Abbreviation,
                Description = StatTypes.StatTypes_Speed_Description
            }
        },
    };

    private static readonly Dictionary<StatCategory, StatTypeData> s_statCategories = new()
    {
        {
            StatCategory.None,
            new()
            {
                Name = StatTypes.StatTypes_None_Abbreviation,
                Abbreviation = StatTypes.StatTypes_None_Abbreviation,
                Description = StatTypes.StatTypes_None_Description
            }
        },
        {
            StatCategory.Attribute,
            new()
            {
                Name = StatTypes.StatTypes_Attribute_Name,
                Abbreviation = StatTypes.StatTypes_Attribute_Abbreviation,
                Description = StatTypes.StatTypes_Attribute_Description
            }
        },
        {
            StatCategory.AttackElement,
            new()
            {
                Name = StatTypes.StatTypes_AttackElement_Name,
                Abbreviation = StatTypes.StatTypes_AttackElement_Abbreviation,
                Description = StatTypes.StatTypes_AttackElement_Description
            }
        },
        {
            StatCategory.ElementResist,
            new()
            {
                Name = StatTypes.StatTypes_ElementResist_Name,
                Abbreviation = StatTypes.StatTypes_ElementResist_Abbreviation,
                Description = StatTypes.StatTypes_ElementResist_Description
            }
        },
        {
            StatCategory.StatusAttack,
            new()
            {
                Name = StatTypes.StatTypes_StatusAttack_Name,
                Abbreviation = StatTypes.StatTypes_StatusAttack_Abbreviation,
                Description = StatTypes.StatTypes_StatusAttack_Description
            }
        },
        {
            StatCategory.StatusResist,
            new()
            {
                Name = StatTypes.StatTypes_StatusResist_Name,
                Abbreviation = StatTypes.StatTypes_StatusResist_Abbreviation,
                Description = StatTypes.StatTypes_StatusResist_Description
            }
        },
        {
            StatCategory.StatusEffect,
            new()
            {
                Name = StatTypes.StatTypes_StatusEffect_Name,
                Abbreviation = StatTypes.StatTypes_StatusEffect_Abbreviation,
                Description = StatTypes.StatTypes_StatusEffect_Description
            }
        },
    };
}
