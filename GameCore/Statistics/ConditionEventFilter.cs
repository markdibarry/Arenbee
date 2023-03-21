using GameCore.Utility;

namespace GameCore.Statistics;

public abstract class ConditionEventFilter : EventFilter<AStats>
{
    protected ConditionEventFilter(AStats stats, Condition condition)
        : base(stats)
    {
        Condition = condition;
    }

    protected Condition Condition { get; set; }
    public bool ConditionMet { get; set; }

    public abstract bool CheckCondition();
}
