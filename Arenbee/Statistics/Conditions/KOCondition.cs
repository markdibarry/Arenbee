using GameCore.Statistics;

namespace Arenbee.Statistics;

public class KOCondition : ConditionEventFilter
{
    public KOCondition(AStats stats, Condition condition)
        : base(stats, condition)
    {
    }

    public void OnHPDepleted()
    {
        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            RaiseConditionChanged();
        }
    }

    public override bool CheckCondition()
    {
        return ((Stats)Source).CurrentHP <= 0;
    }

    public override void SubscribeEvents()
    {
        ((Stats)Source).HPDepleted += OnHPDepleted;
    }

    public override void UnsubscribeEvents()
    {
        ((Stats)Source).HPDepleted -= OnHPDepleted;
    }
}
