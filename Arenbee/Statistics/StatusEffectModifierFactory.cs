using GameCore.Enums;
using GameCore.Statistics;

namespace Arenbee.Statistics;
public class StatusEffectModifierFactory : IStatusEffectModifierFactory
{
    public Modifier GetStatusEffectModifier(int statusEffectType)
    {
        return (StatusEffectType)statusEffectType switch
        {
            StatusEffectType.KO => new Modifier(
                statType: (int)StatType.KO,
                modOperator: ModOp.One,
                value: 1,
                activationConditions: null,
                removalConditions: null,
                isHidden: false),
            StatusEffectType.Poison => new Modifier(
                statType: (int)StatType.Poison,
                modOperator: ModOp.One,
                value: 1,
                activationConditions: null,
                removalConditions: new Godot.Collections.Array<Condition>()
                {
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.Timed,
                        Target = 10,
                        LogicOp = LogicOp.Or
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.HPPercent,
                        TargetValue = 10,
                        LogicOp = LogicOp.Or
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.KO
                    }
                },
                isHidden: false),
            StatusEffectType.Burn => new Modifier(
                statType: (int)StatType.Burn,
                modOperator: ModOp.One,
                value: 1,
                activationConditions: null,
                removalConditions: new Godot.Collections.Array<Condition>()
                {
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.Timed,
                        Target = 10,
                        LogicOp = LogicOp.Or
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.HPPercent,
                        TargetValue = 10,
                        LogicOp = LogicOp.Or
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.KO
                    }
                },
                isHidden: false),
            StatusEffectType.Paralysis => new Modifier(
                statType: (int)StatType.Burn,
                modOperator: ModOp.One,
                value: 1,
                activationConditions: null,
                removalConditions: new Godot.Collections.Array<Condition>()
                {
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.Timed,
                        Target = 10,
                        LogicOp = LogicOp.Or
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.KO
                    }
                },
                isHidden: false),
            StatusEffectType.None or
            StatusEffectType.Freeze or
            StatusEffectType.Zombie or
            _ => throw new System.NotImplementedException()
        };
    }
}
