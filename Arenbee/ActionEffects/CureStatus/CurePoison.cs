﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;

namespace Arenbee.ActionEffects;

public class CurePoison : IActionEffect
{
    public bool IsActionSequence => false;
    public int TargetType => (int)ActionEffects.TargetType.PartyMember;

    public bool CanUse(BaseActor? user, IList<BaseActor> targets, int actionType, int value1, int value2)
    {
        if (targets.Count != 1)
            return false;
        Stats stats = (Stats)targets[0].Stats;
        return stats.HasStatusEffect((int)StatusEffectType.Poison) && !stats.HasNoHP;
    }

    public Task Use(BaseActor? user, IList<BaseActor> targets, int actionType, int value1, int value2)
    {
        Stats stats = (Stats)targets[0].Stats;
        stats.RemoveModsByType(StatType.Poison);
        return Task.CompletedTask;
    }
}
