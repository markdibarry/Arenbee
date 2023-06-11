using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

public partial class StatusCondition : Condition
{
    public StatusCondition() { }

    public StatusCondition(StatusCondition condition)
        : base(condition)
    {
        TargetValue = condition.TargetValue;
    }

    public override int ConditionType => (int)Statistics.ConditionType.Status;
    [Export] public int TargetValue { get; set; }

    public override StatusCondition Clone() => new(this);

    protected override bool CheckIfConditionMet(BaseStats stats) => stats.HasStatusEffect(TargetValue);

    public override void SubscribeEvents(BaseStats stats) => stats.StatusEffectChanged += OnStatusEffectChanged;

    public override void UnsubscribeEvents(BaseStats stats) => stats.StatusEffectChanged -= OnStatusEffectChanged;

    private void OnStatusEffectChanged(int statusEffectType, ModChangeType changeType)
    {
        if (TargetValue != statusEffectType)
            return;
        RaiseConditionUpdated();
    }
}
