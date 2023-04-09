using System;
using GameCore.Enums;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public partial class TimedCondition : Condition
{
    public TimedCondition() { }

    public TimedCondition(
        ConditionResultType resultType,
        LogicOp additionalLogicOp,
        float startValue,
        float targetValue,
        float currentValue,
        Condition? additionalCondition)
            : base(resultType, additionalLogicOp, additionalCondition)
    {
        StartValue = startValue;
        CurrentValue = currentValue;
        TargetValue = targetValue;
    }

    public override int ConditionType => (int)Statistics.ConditionType.Timed;
    [Export] public float StartValue { get; set; }
    [Export] public float TargetValue { get; set; }
    [Export] public float CurrentValue { get; set; }

    public override TimedCondition Clone()
    {
        return new TimedCondition(ResultType, AdditionalLogicOp, StartValue, TargetValue, CurrentValue, AdditionalCondition);
    }

    public override void Reset()
    {
        CurrentValue = StartValue;
        base.Reset();
    }

    protected override bool CheckCondition()
    {
        return CurrentValue == TargetValue;
    }

    protected override void SubscribeEvents()
    {
        Stats.Processed += OnProcessed;
    }

    protected override void UnsubscribeEvents()
    {
        Stats.Processed -= OnProcessed;
    }

    private void OnProcessed(double amount)
    {
        if (CurrentValue != TargetValue)
        {
            if (CurrentValue < TargetValue)
                CurrentValue = Math.Min(CurrentValue + (float)amount, TargetValue);
            else
                CurrentValue = Math.Max(CurrentValue - (float)amount, TargetValue);
        }

        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            ConditionChangedCallback?.Invoke();
        }
    }
}
