using GameCore.Statistics;
using Godot;

namespace Arenbee.Statistics;

[GlobalClass]
public partial class KOCondition : Condition
{
    public KOCondition() { }

    public KOCondition(KOCondition condition)
        : base(condition)
    {
    }

    public override int ConditionType => (int)Statistics.ConditionType.KO;

    public override KOCondition Clone() => new(this);

    public override void SubscribeEvents(BaseStats stats) => ((Stats)stats).HPDepleted += OnHPDepleted;

    public override void UnsubscribeEvents(BaseStats stats) => ((Stats)stats).HPDepleted -= OnHPDepleted;

    protected override bool CheckIfConditionMet(BaseStats stats) => ((Stats)stats).CurrentHP <= 0;

    private void OnHPDepleted() => RaiseConditionUpdated();
}
