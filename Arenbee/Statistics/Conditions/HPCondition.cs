using GameCore.Statistics;
using GameCore.Utility;

namespace Arenbee.Statistics;

public class HPCondition : ConditionEventFilter
{
    public HPCondition(AStats stats, Condition condition)
        : base(stats, condition)
    {
    }

    public void OnDamageReceived(ADamageResult damageResult)
    {
        RaiseConditionChanged();
    }

    public override bool CheckCondition()
    {
        int percentMaxHP = (int)(((Stats)Source).MaxHP * Condition.TargetValue * 0.01);
        return MathI.Compare(Condition.CompareOp, ((Stats)Source).CurrentHP, percentMaxHP);
    }

    public override void SubscribeEvents()
    {
        ((Stats)Source).DamageReceived += OnDamageReceived;
    }

    public override void UnsubscribeEvents()
    {
        ((Stats)Source).DamageReceived -= OnDamageReceived;
    }
}
