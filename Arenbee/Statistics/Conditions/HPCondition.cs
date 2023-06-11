using GameCore.Statistics;
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

    public override HPCondition Clone() => new(this);

    protected override bool CheckIfConditionMet(BaseStats baseStats)
    {
        Stats stats = (Stats)baseStats;
        int percentMaxHP = (int)(stats.MaxHP * TargetValue * 0.01);
        return CompareOp.Compare(stats.CurrentHP, percentMaxHP);
    }

    public override void SubscribeEvents(BaseStats stats) => stats.DamageReceived += OnDamageReceived;

    public override void UnsubscribeEvents(BaseStats stats) => stats.DamageReceived -= OnDamageReceived;

    private void OnDamageReceived(IDamageResult damageResult) => RaiseConditionUpdated();
}
