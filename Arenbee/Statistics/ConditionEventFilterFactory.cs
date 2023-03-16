using GameCore.Statistics;

namespace Arenbee.Statistics;

public class ConditionEventFilterFactory : IConditionEventFilterFactory
{
    public ConditionEventFilter GetEventFilter(AStats stats, Condition condition)
    {
        return (ConditionType)condition.ConditionType switch
        {
            ConditionType.Timed => new TimedCondition(stats, condition),
            ConditionType.Status => new StatusCondition(stats, condition),
            ConditionType.KO => new KOCondition(stats, condition),
            ConditionType.HPPercent => new HPCondition(stats, condition),
            ConditionType.None or
            _ => throw new System.NotImplementedException(),
        };
    }
}
