﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using GameCore.Enums;
using GameCore.Utility;
using Godot;
using Gictionary = Godot.Collections.Dictionary;

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
        Conditions = new(mod.Conditions?.Select(x => x.Clone()));
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

    public void InitConditions(AStats stats)
    {
        foreach (Condition condition in Conditions)
            condition.SetStats(stats);
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
            condition.Unsubscribe();
    }

    private bool CheckConditions(ConditionResultType resultType)
    {
        foreach (Condition condition in Conditions)
        {
            if (condition.ResultType != ConditionResultType.RemoveOrDeactivate && condition.ResultType != resultType)
                continue;
            if (condition.CheckConditions())
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
        return condition.ResultType switch
        {
            ConditionResultType.Remove => ConditionRemovalHandler,
            ConditionResultType.Deactivate => ConditionActivationHandler,
            _ => SourceType == SourceType.Independent ? ConditionRemovalHandler : ConditionActivationHandler
        };
    }

    public override Godot.Collections.Array<Gictionary> _GetPropertyList()
    {
        return Locator.StatTypeDB.GetStatPropertyList(_statType);
    }
}
