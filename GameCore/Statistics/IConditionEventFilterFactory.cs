namespace GameCore.Statistics;
public interface IConditionEventFilterFactory
{
    ConditionEventFilter GetEventFilter(AStats stats, Condition condition);
}
