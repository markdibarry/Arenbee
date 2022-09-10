using System;
using System.Collections.Generic;

namespace GameCore.Statistics;

public class StatusEffectOffs : StatDict<StatusEffectOff>
{
    public StatusEffectOffs()
    {
        StatType = StatType.StatusEffectOff;
    }

    public StatusEffectOffs(StatusEffectOffs offs)
        : this()
    {
        foreach (var pair in offs.StatsDict)
            StatsDict[pair.Key] = new StatusEffectOff(pair.Key, pair.Value);
    }

    public List<Modifier> GetModifiers()
    {
        var result = new List<Modifier>();
        foreach (var pair in StatsDict)
        {
            if (pair.Value.ModifiedValue != 0)
                result.Add(new Modifier(StatType.StatusEffect, pair.Key, ModOperator.Add, pair.Value.ModifiedValue, pair.Value.Chance));
        }
        return result;
    }

    protected override StatusEffectOff GetNewStat(int type)
    {
        return new StatusEffectOff(type);
    }
}

public class StatusEffectOff : Stat
{
    public StatusEffectOff(int type)
        : base(type)
    { }

    public StatusEffectOff(int type, StatusEffectOff statusEffectOff)
        : base(type, statusEffectOff)
    { }

    public int Chance
    {
        get
        {
            int chance = 0;
            foreach (var mod in Modifiers)
                chance += mod.Chance;

            return Math.Min(chance, 100);
        }
    }
    public StatusEffectType StatusEffectType
    {
        get { return (StatusEffectType)SubType; }
        set { SubType = (int)value; }
    }

    public override int CalculateStat(bool ignoreHidden = false)
    {
        int result = BaseValue;

        foreach (var mod in Modifiers)
        {
            if (ignoreHidden && mod.IsHidden)
                continue;
            result = mod.Apply(result);
        }

        return result;
    }
}
