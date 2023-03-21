using System;
using GameCore.Enums;
using Godot;

namespace GameCore.Statistics;

public partial class Condition : Resource
{
    public Condition()
    {
    }

    public Condition(Condition condition)
        : this(conditionType: condition.ConditionType,
              target: condition.Target,
              startValue: condition.StartValue,
              currentValue: condition.CurrentValue,
              targetValue: condition.TargetValue,
              compareOp: condition.CompareOp,
              resultType: condition.ResultType,
              additionalLogicOp: condition.AdditionalLogicOp,
              additionalCondition: condition.AdditionalCondition)
    {
    }

    public Condition(
        int conditionType,
        int target,
        float startValue,
        float currentValue,
        float targetValue,
        CompareOp compareOp,
        ConditionResultType resultType,
        LogicOp additionalLogicOp,
        Condition? additionalCondition)
    {
        ConditionType = conditionType;
        Target = target;
        StartValue = startValue;
        CurrentValue = currentValue;
        TargetValue = targetValue;
        CompareOp = compareOp;
        ResultType = resultType;
        AdditionalLogicOp = additionalLogicOp;
        AdditionalCondition = additionalCondition == null ? null : new(additionalCondition);
    }

    [Export] public int ConditionType { get; set; }
    [Export] public int Target { get; set; }
    [Export] public float StartValue { get; set; }
    [Export] public float CurrentValue { get; set; }
    [Export] public CompareOp CompareOp { get; set; }
    [Export] public float TargetValue { get; set; }
    [Export] public ConditionResultType ResultType { get; set; }
    [Export] public Condition? AdditionalCondition { get; set; }
    [Export] public LogicOp AdditionalLogicOp { get; set; }
    public ConditionEventFilter? EventFilter { get; set; }

    public bool CheckCondition()
    {
        if (EventFilter == null)
            return false;
        EventFilter.ConditionMet = EventFilter.CheckCondition();
        if (EventFilter.ConditionMet)
        {
            if (AdditionalCondition?.AdditionalLogicOp == LogicOp.And)
                return AdditionalCondition.CheckCondition();
            return true;
        }
        else
        {
            if (AdditionalCondition?.AdditionalLogicOp == LogicOp.Or)
                return AdditionalCondition.CheckCondition();
            return false;
        }
    }

    public virtual void Reset()
    {
        CurrentValue = StartValue;
        AdditionalCondition?.Reset();
    }

    public void Subscribe(Action handler)
    {
        if (EventFilter == null)
            return;
        EventFilter.SubscribeEvents();
        EventFilter.ConditionChanged += handler;
        AdditionalCondition?.Subscribe(handler);
    }

    public void Unsubscribe(Action handler)
    {
        if (EventFilter == null)
            return;
        EventFilter.UnsubscribeEvents();
        EventFilter.ConditionChanged -= handler;
        AdditionalCondition?.Unsubscribe(handler);
    }
}
