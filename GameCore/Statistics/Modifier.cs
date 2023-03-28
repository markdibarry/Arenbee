using System;
using System.Linq;
using System.Text.Json.Serialization;
using GameCore.Enums;
using GameCore.Utility;
using Godot;

namespace GameCore.Statistics;

public partial class Modifier : Resource
{
    public Modifier() { }

    [JsonConstructor]
    public Modifier(
        int statType,
        ModOp op,
        int value,
        SourceType sourceType = default,
        Godot.Collections.Array<Condition>? conditions = null,
        bool isHidden = false)
    {
        StatType = statType;
        Op = op;
        Value = value;
        SourceType = sourceType;
        Conditions = conditions ?? new();
        IsHidden = isHidden;
    }

    public Modifier(Modifier mod)
    {
        StatType = mod.StatType;
        Op = mod.Op;
        IsHidden = mod.IsHidden;
        SourceType = mod.SourceType;
        Value = mod.Value;
        Conditions = new(mod.Conditions?.Select(x => new Condition(x)));
    }

    private int _statType;
    public int StatType
    {
        get => _statType;
        set
        {
            _statType = value;
            NotifyPropertyListChanged();
        }
    }
    public int Value { get; set; }
    [Export] public bool IsHidden { get; set; }
    [Export] public ModOp Op { get; set; }
    [Export] public SourceType SourceType { get; set; }
    [Export] public Godot.Collections.Array<Condition> Conditions { get; set; } = new();
    public bool IsActive { get; set; }
    public event Action<Modifier>? RemovalConditionMet;
    public event Action<Modifier>? ActivationConditionMet;

    // TODO Add special case handling i.e. +5% for every 100 enemies killed
    public int Apply(int baseValue) => MathI.Compute(Op, baseValue, Value);

    public void InitConditions(AStats stats, IConditionEventFilterFactory factory)
    {
        foreach (Condition condition in Conditions)
            condition.EventFilter = factory.GetEventFilter(stats, condition);
    }

    public void ResetConditions()
    {
        if (Conditions == null)
            return;
        foreach (Condition condition in Conditions)
            condition.Reset();
    }

    public bool ShouldDeactivate() => CheckConditions(ConditionResultType.Deactivate);

    public bool ShouldRemove() => CheckConditions(ConditionResultType.Remove);

    public void SubscribeConditions()
    {
        foreach (Condition condition in Conditions)
            condition.Subscribe(GetHandler(condition));
    }

    public void UnsubscribeConditions()
    {
        foreach (Condition condition in Conditions)
            condition.Unsubscribe(GetHandler(condition));
    }

    private bool CheckConditions(ConditionResultType resultType)
    {
        foreach (Condition condition in Conditions)
        {
            if (condition.ResultType != ConditionResultType.RemoveOrDeactivate && condition.ResultType != resultType)
                continue;
            if (condition.CheckCondition())
                return true;
        }
        return false;
    }

    private void ConditionActivationHandler()
    {
        bool isActive = !ShouldDeactivate();
        if (IsActive != isActive)
        {
            IsActive = isActive;
            ActivationConditionMet?.Invoke(this);
        }
    }

    private void ConditionRemovalHandler()
    {
        if (ShouldRemove())
            RemovalConditionMet?.Invoke(this);
    }

    private Action GetHandler(Condition condition)
    {
        Action handler;
        if (condition.ResultType == ConditionResultType.Remove)
            handler = ConditionRemovalHandler;
        else if (condition.ResultType == ConditionResultType.Deactivate)
            handler = ConditionActivationHandler;
        else
            handler = SourceType == SourceType.Independent ? ConditionRemovalHandler : ConditionActivationHandler;
        return handler;
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        return Locator.StatTypeDB.GetStatPropertyList(_statType);
    }
}

public enum SourceType
{
    //Innate,
    Dependent,
    Independent
}
