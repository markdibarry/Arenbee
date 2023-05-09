using System;
using System.Diagnostics.CodeAnalysis;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public partial class TimedCondition : Condition
{
    public TimedCondition() { }

    public TimedCondition(TimedCondition condition)
            : base(condition)
    {
        StartValue = condition.StartValue;
        CurrentValue = condition.CurrentValue;
        TargetValue = condition.TargetValue;
    }

    public override int ConditionType => (int)Statistics.ConditionType.Timed;
    [Export] public float StartValue { get; set; }
    [Export] public float TargetValue { get; set; }
    [Export] public float CurrentValue { get; set; }

    public override TimedCondition Clone() => new(this);

    public override void Reset()
    {
        CurrentValue = StartValue;
        base.Reset();
    }

    protected override bool CheckCondition() => CurrentValue == TargetValue;

    protected override void SubscribeEvents() => Stats.Processed += OnProcessed;

    protected override void UnsubscribeEvents() => Stats.Processed -= OnProcessed;

    private void OnProcessed(double amount)
    {
        if (CurrentValue != TargetValue)
        {
            if (CurrentValue < TargetValue)
                CurrentValue = Math.Min(CurrentValue + (float)amount, TargetValue);
            else
                CurrentValue = Math.Max(CurrentValue - (float)amount, TargetValue);
        }

        UpdateCondition();
    }
}
