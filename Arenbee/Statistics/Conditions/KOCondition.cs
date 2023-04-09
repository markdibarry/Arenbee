using GameCore.Enums;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public partial class KOCondition : Condition
{
    public KOCondition() { }

    public KOCondition(ConditionResultType resultType, LogicOp additionalLogicOp, Condition? additionalCondition)
            : base(resultType, additionalLogicOp, additionalCondition)
    {
    }

    public override int ConditionType => (int)Statistics.ConditionType.KO;

    protected override bool CheckCondition()
    {
        return ((Stats)Stats).CurrentHP <= 0;
    }

    protected override void SubscribeEvents()
    {
        ((Stats)Stats).HPDepleted += OnHPDepleted;
    }

    protected override void UnsubscribeEvents()
    {
        ((Stats)Stats).HPDepleted -= OnHPDepleted;
    }

    public override KOCondition Clone()
    {
        return new KOCondition(ResultType, AdditionalLogicOp, AdditionalCondition?.Clone());
    }

    private void OnHPDepleted()
    {
        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            ConditionChangedCallback?.Invoke();
        }
    }
}
