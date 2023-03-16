using System;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public class TimedCondition : ConditionEventFilter
{
    public TimedCondition(AStats stats, Condition condition)
        : base(stats, condition)
    {
    }

    public void OnProcessed(double amount)
    {
        if (Condition.CurrentValue != Condition.TargetValue)
        {
            if (Condition.CurrentValue < Condition.TargetValue)
                Condition.CurrentValue = Math.Min(Condition.CurrentValue + (int)amount, Condition.TargetValue);
            else
                Condition.CurrentValue = Math.Max(Condition.CurrentValue - (int)amount, Condition.TargetValue);
        }

        RaiseConditionChanged();
    }

    public override bool CheckCondition()
    {
        return Condition.CurrentValue == Condition.TargetValue;
    }

    public override void SubscribeEvents()
    {
        Source.Processed += OnProcessed;
    }

    public override void UnsubscribeEvents()
    {
        Source.Processed -= OnProcessed;
    }
}
