using System;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

[GlobalClass]
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

    protected override bool CheckIfConditionMet(BaseStats stats) => CurrentValue == TargetValue;

    public override void SubscribeEvents(BaseStats stats) => stats.Processed += OnProcessed;

    public override void UnsubscribeEvents(BaseStats stats) => stats.Processed -= OnProcessed;

    private void OnProcessed(double amount)
    {
        if (CurrentValue != TargetValue)
        {
            if (CurrentValue < TargetValue)
                CurrentValue = Math.Min(CurrentValue + (float)amount, TargetValue);
            else
                CurrentValue = Math.Max(CurrentValue - (float)amount, TargetValue);
        }

        RaiseConditionUpdated();
    }
}
