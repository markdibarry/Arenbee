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
                Condition.CurrentValue = Math.Min(Condition.CurrentValue + (float)amount, Condition.TargetValue);
            else
                Condition.CurrentValue = Math.Max(Condition.CurrentValue - (float)amount, Condition.TargetValue);
        }

        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            RaiseConditionChanged();
        }
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
