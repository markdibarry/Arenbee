using GameCore.Enums;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Statistics;

public partial class HPCondition : Condition
{
    public HPCondition() { }

    public HPCondition(HPCondition condition)
        : base(condition)
    {
        CompareOp = condition.CompareOp;
        TargetValue = condition.TargetValue;
    }

    public override int ConditionType => (int)Statistics.ConditionType.HPPercent;
    [Export] public CompareOp CompareOp { get; set; }
    [Export] public int TargetValue { get; set; }
    protected override Stats Stats => (Stats)StatsInternal;

    public override HPCondition Clone() => new(this);

    protected override bool CheckCondition()
    {
        int percentMaxHP = (int)(Stats.MaxHP * TargetValue * 0.01);
        return MathI.Compare(CompareOp, Stats.CurrentHP, percentMaxHP);
    }

    protected override void SubscribeEvents() => Stats.DamageReceived += OnDamageReceived;

    protected override void UnsubscribeEvents() => Stats.DamageReceived -= OnDamageReceived;

    private void OnDamageReceived(ADamageResult damageResult) => UpdateCondition();
}
