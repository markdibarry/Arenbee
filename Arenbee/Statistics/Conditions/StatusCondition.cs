using GameCore.Enums;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public class StatusCondition : ConditionEventFilter
{
    public StatusCondition(AStats stats, Condition condition)
        : base(stats, condition)
    {
    }

    public void OnStatusEffectChanged(int statusEffectType, ChangeType changeType)
    {
        if (Condition.TargetValue != statusEffectType)
            return;
        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            RaiseConditionChanged();
        }
    }

    public override bool CheckCondition()
    {
        return Source.HasStatusEffect((int)Condition.TargetValue);
    }

    public override void SubscribeEvents()
    {
        Source.StatusEffectChanged += OnStatusEffectChanged;
    }

    public override void UnsubscribeEvents()
    {
        Source.StatusEffectChanged -= OnStatusEffectChanged;
    }
}
