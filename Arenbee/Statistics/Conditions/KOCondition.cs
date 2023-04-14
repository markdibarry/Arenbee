using GameCore.Statistics;

namespace Arenbee.Statistics;

public partial class KOCondition : Condition
{
    public KOCondition() { }

    public KOCondition(KOCondition condition)
        : base(condition)
    {
    }

    public override int ConditionType => (int)Statistics.ConditionType.KO;
    protected override Stats Stats => (Stats)base.Stats;

    public override KOCondition Clone() => new(this);

    protected override bool CheckCondition() => Stats.CurrentHP <= 0;

    protected override void SubscribeEvents() => Stats.HPDepleted += OnHPDepleted;

    protected override void UnsubscribeEvents() => Stats.HPDepleted -= OnHPDepleted;

    private void OnHPDepleted() => UpdateCondition();
}
