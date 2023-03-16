using GameCore.Statistics;

namespace Arenbee.Statistics;

public class KOCondition : ConditionEventFilter
{
    public KOCondition(AStats stats, Condition condition)
        : base(stats, condition)
    {
    }

    public override bool CheckCondition()
    {
        return ((Stats)Source).CurrentHP <= 0;
    }

    public override void SubscribeEvents()
    {
        ((Stats)Source).HPDepleted += RaiseConditionChanged;
    }

    public override void UnsubscribeEvents()
    {
        ((Stats)Source).HPDepleted -= RaiseConditionChanged;
    }
}
