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
                op: ModOp.One,
                value: 1,
                conditions: null,
                isHidden: false),
            StatusEffectType.Poison => new Modifier(
                statType: (int)StatType.Poison,
                op: ModOp.One,
                value: 1,
                conditions: new Godot.Collections.Array<Condition>()
                {
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.Timed,
                        TargetValue = 10,
                        ResultType = ConditionResultType.Remove
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.HPPercent,
                        CompareOp = CompareOp.LessEquals,
                        TargetValue = 10,
                        ResultType = ConditionResultType.RemoveOrDeactivate,
                        AdditionalLogicOp = LogicOp.Or,
                        AdditionalCondition = new Condition()
                        {
                            ConditionType = (int)ConditionType.KO
                        }
                    }
                },
                isHidden: false),
            StatusEffectType.Burn => new Modifier(
                statType: (int)StatType.Burn,
                op: ModOp.One,
                value: 1,
                conditions: new Godot.Collections.Array<Condition>()
                {
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.Timed,
                        TargetValue = 10,
                        ResultType = ConditionResultType.Remove
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.HPPercent,
                        CompareOp = CompareOp.LessEquals,
                        TargetValue = 10,
                        ResultType = ConditionResultType.RemoveOrDeactivate,
                        AdditionalLogicOp = LogicOp.Or,
                        AdditionalCondition = new Condition()
                        {
                            ConditionType = (int)ConditionType.KO
                        }
                    }
                },
                isHidden: false),
            StatusEffectType.Paralysis => new Modifier(
                statType: (int)StatType.Burn,
                op: ModOp.One,
                value: 1,
                conditions: new Godot.Collections.Array<Condition>()
                {
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.Timed,
                        TargetValue = 10,
                        ResultType = ConditionResultType.Remove
                    },
                    new Condition()
                    {
                        ConditionType = (int)ConditionType.KO,
                        ResultType = ConditionResultType.RemoveOrDeactivate
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
