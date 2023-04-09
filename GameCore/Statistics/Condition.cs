using System;
using System.Text.Json.Serialization;
using GameCore.Enums;
using Godot;

namespace GameCore.Statistics;

[JsonConverter(typeof(ConditionConverter))]
public abstract partial class Condition : Resource
{
    protected Condition() { }

    protected Condition(Condition condition)
        : this(condition.ResultType, condition.AdditionalLogicOp, condition.AdditionalCondition)
    {
    }

    protected Condition(
        ConditionResultType resultType,
        LogicOp additionalLogicOp,
        Condition? additionalCondition)
    {
        ResultType = resultType;
        AdditionalLogicOp = additionalLogicOp;
        AdditionalCondition = additionalCondition?.Clone();
    }

    public abstract int ConditionType { get; }
    [Export] public ConditionResultType ResultType { get; set; }
    [Export, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(ConditionConverter))]
    public Condition? AdditionalCondition { get; set; }
    [Export, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LogicOp AdditionalLogicOp { get; set; }
    [JsonIgnore] public Action? ConditionChangedCallback { get; set; }
    [JsonIgnore] protected bool ConditionMet { get; set; }
    [JsonIgnore] protected AStats Stats { get; set; } = null!;

    public void SetStats(AStats stats)
    {
        Stats = stats;
        AdditionalCondition?.SetStats(stats);
    }

    public bool CheckConditions()
    {
        ConditionMet = CheckCondition();
        if (ConditionMet)
        {
            if (AdditionalCondition?.AdditionalLogicOp == LogicOp.And)
                return AdditionalCondition.CheckConditions();
            return true;
        }
        else
        {
            if (AdditionalCondition?.AdditionalLogicOp == LogicOp.Or)
                return AdditionalCondition.CheckConditions();
            return false;
        }
    }

    public virtual void Reset()
    {
        AdditionalCondition?.Reset();
    }

    public void Subscribe(Action handler)
    {
        SubscribeEvents();
        ConditionChangedCallback = handler;
        AdditionalCondition?.Subscribe(handler);
    }

    public void Unsubscribe()
    {
        UnsubscribeEvents();
        ConditionChangedCallback = null;
        AdditionalCondition?.Unsubscribe();
    }

    public abstract Condition Clone();
    protected abstract bool CheckCondition();
    protected abstract void SubscribeEvents();
    protected abstract void UnsubscribeEvents();
}
