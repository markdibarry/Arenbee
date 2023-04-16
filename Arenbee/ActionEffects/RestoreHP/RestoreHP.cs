﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;

namespace Arenbee.ActionEffects;

public class RestoreHP : IActionEffect
{
    public int TargetType => (int)ActionEffects.TargetType.PartyMember;

    public bool CanUse(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        if (targets.Count != 1)
            return false;
        Stats stats = (Stats)targets[0].Stats;
        return !stats.HasFullHP && !stats.HasNoHP;
    }

    public Task Use(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        AActor target = targets[0];
        DamageRequest actionData = new()
        {
            SourceName = target.Name,
            ActionType = (ActionType)actionType,
            Value = value1 * -1,
            ElementType = ElementType.Healing
        };

        target.Stats.ReceiveDamageRequest(actionData);
        return Task.CompletedTask;
    }
}
