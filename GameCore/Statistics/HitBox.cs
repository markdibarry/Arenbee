﻿using System;
using GameCore.Actors;
using GameCore.Enums;
using Godot;

namespace GameCore.Statistics;

public partial class HitBox : AreaBox
{
    public HitBox()
    {
        ActionData = new ActionData()
        {
            ActionType = ActionType.Environment,
            ElementDamage = ElementType.None,
            SourceName = Name,
            StatusEffectDamage = StatusEffectType.None,
            Value = InitialValue
        };
        GetActionData = () =>
        {
            var actionData = ActionData;
            actionData.SourcePosition = GlobalPosition;
            return actionData;
        };
    }

    [Export]
    public int InitialValue { get; private set; } = 1;
    public ActionData ActionData { get; set; }
    public Func<ActionData> GetActionData { get; set; }

    public void SetBasicMeleeBox(AActorBody actor)
    {
        ActionData actionData = ActionData;
        actionData.SourceName = actor.Name;
        actionData.ActionType = ActionType.Melee;
        GetActionData = () =>
        {
            actionData.SourcePosition = GlobalPosition;
            actionData.ElementDamage = actor.Actor.Stats.ElementOffs.CurrentElement;
            actionData.StatusEffects = actor.Actor.Stats.StatusEffectOffs.GetModifiers();
            actionData.Value = actor.Actor.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue;
            return actionData;
        };
    }
}
