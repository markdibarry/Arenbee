using GameCore.Enums;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public partial class StatusCondition : Condition
{
    public StatusCondition() { }

    public StatusCondition(
        ConditionResultType resultType,
        LogicOp additionalLogicOp,
        int targetValue,
        Condition? additionalCondition)
            : base(resultType, additionalLogicOp, additionalCondition)
    {
        TargetValue = targetValue;
    }

    public override int ConditionType => (int)Statistics.ConditionType.Status;
    [Export] public int TargetValue { get; set; }

    public override StatusCondition Clone()
    {
        return new StatusCondition(
            ResultType,
            AdditionalLogicOp,
            TargetValue,
            AdditionalCondition?.Clone());
    }

    protected override bool CheckCondition()
    {
        return Stats.HasStatusEffect(TargetValue);
    }

    protected override void SubscribeEvents()
    {
        Stats.StatusEffectChanged += OnStatusEffectChanged;
    }

    protected override void UnsubscribeEvents()
    {
        Stats.StatusEffectChanged -= OnStatusEffectChanged;
    }

    private void OnStatusEffectChanged(int statusEffectType, ModChangeType changeType)
    {
        if (TargetValue != statusEffectType)
            return;
        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            ConditionChangedCallback?.Invoke();
        }
    }
}
