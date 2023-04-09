using GameCore.Enums;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Statistics;

public partial class HPCondition : Condition
{
    public HPCondition() { }

    public HPCondition(
        ConditionResultType resultType,
        LogicOp additionalLogicOp,
        CompareOp compareOp,
        int targetValue,
        Condition? additionalCondition)
            : base(resultType, additionalLogicOp, additionalCondition)
    {
        CompareOp = compareOp;
        TargetValue = targetValue;
    }

    public override int ConditionType => (int)Statistics.ConditionType.HPPercent;
    [Export] public CompareOp CompareOp { get; set; }
    [Export] public int TargetValue { get; set; }

    protected override bool CheckCondition()
    {
        int percentMaxHP = (int)(((Stats)Stats).MaxHP * TargetValue * 0.01);
        return MathI.Compare(CompareOp, ((Stats)Stats).CurrentHP, percentMaxHP);
    }

    protected override void SubscribeEvents()
    {
        ((Stats)Stats).DamageReceived += OnDamageReceived;
    }

    protected override void UnsubscribeEvents()
    {
        ((Stats)Stats).DamageReceived -= OnDamageReceived;
    }

    public override HPCondition Clone()
    {
        return new HPCondition(
            ResultType,
            AdditionalLogicOp,
            CompareOp,
            TargetValue,
            AdditionalCondition?.Clone());
    }

    private void OnDamageReceived(ADamageResult damageResult)
    {
        bool result = CheckCondition();
        if (result != ConditionMet)
        {
            ConditionMet = result;
            ConditionChangedCallback?.Invoke();
        }
    }
}
