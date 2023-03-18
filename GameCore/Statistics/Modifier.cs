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
        ModOp modOperator,
        int value,
        Godot.Collections.Array<Condition>? activationConditions = null,
        Godot.Collections.Array<Condition>? removalConditions = null,
        bool isHidden = false)
    {
        StatType = statType;
        Op = modOperator;
        Value = value;
        ActivationConditions = activationConditions ?? new();
        RemovalConditions = removalConditions ?? new();
        IsHidden = isHidden;
    }

    public Modifier(Modifier mod)
    {
        StatType = mod.StatType;
        Op = mod.Op;
        IsHidden = mod.IsHidden;
        Value = mod.Value;
        ActivationConditions = new(mod.ActivationConditions?.Select(x => new Condition(x)));
        RemovalConditions = new(mod.RemovalConditions?.Select(x => new Condition(x)));
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
    [Export] public Godot.Collections.Array<Condition> ActivationConditions { get; set; } = new();
    [Export] public Godot.Collections.Array<Condition> RemovalConditions { get; set; } = new();
    public bool IsActive { get; set; }
    public event System.Action<Modifier>? RemovalConditionMet;

    // TODO Add special case handling i.e. +5% for every 100 enemies killed
    public int Apply(int baseValue) => MathI.Compute(Op, baseValue, Value);

    public void InitActivationConditions(AStats stats, IConditionEventFilterFactory factory)
    {
        InitConditions(stats, factory, ActivationConditions);
    }

    public void InitRemovalConditions(AStats stats, IConditionEventFilterFactory factory)
    {
        InitConditions(stats, factory, RemovalConditions);
    }

    public void ResetConditions()
    {
        if (RemovalConditions == null)
            return;
        foreach (Condition condition in RemovalConditions)
            condition.Reset();
    }

    public bool ShouldActivate()
    {
        if (!ActivationConditions.Any())
            return true;
        return CheckConditions(ActivationConditions);
    }

    public bool ShouldRemove() => CheckConditions(RemovalConditions);

    public void SubscribeActivationConditions()
    {
        SubscribeConditions(ActivationConditions, ConditionActivationHandler);
    }

    public void SubscribeRemovalConditions()
    {
        SubscribeConditions(RemovalConditions, ConditionRemovalHandler);
    }

    public void UnsubscribeActivationConditions()
    {
        UnsubscribeConditions(ActivationConditions, ConditionActivationHandler);
    }

    public void UnsubscribeRemovalConditions()
    {
        UnsubscribeConditions(RemovalConditions, ConditionRemovalHandler);
    }

    private static bool CheckConditions(Godot.Collections.Array<Condition> conditions)
    {
        foreach (Condition condition in conditions)
        {
            if (condition.EventFilter == null)
                continue;
            if (condition.EventFilter.CheckCondition())
            {
                if (condition.LogicOp != LogicOp.And)
                    return true;
            }
            else
            {
                if (condition.LogicOp != LogicOp.Or)
                    return false;
            }
        }
        return false;
    }

    private void ConditionActivationHandler()
    {
        IsActive = ShouldActivate();
    }

    private void ConditionRemovalHandler()
    {
        if (ShouldRemove())
            RemovalConditionMet?.Invoke(this);
    }

    private static void InitConditions(AStats stats, IConditionEventFilterFactory factory, Godot.Collections.Array<Condition> conditions)
    {
        foreach (Condition condition in conditions)
            condition.EventFilter = factory.GetEventFilter(stats, condition);
    }

    private static void SubscribeConditions(Godot.Collections.Array<Condition> conditions, System.Action handler)
    {
        foreach (Condition condition in conditions)
        {
            if (condition.EventFilter == null)
                continue;
            condition.EventFilter.ConditionChanged += handler;
        }
    }

    private static void UnsubscribeConditions(Godot.Collections.Array<Condition> conditions, System.Action handler)
    {
        foreach (Condition condition in conditions)
        {
            if (condition.EventFilter == null)
                continue;
            condition.EventFilter.ConditionChanged -= handler;
        }
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
