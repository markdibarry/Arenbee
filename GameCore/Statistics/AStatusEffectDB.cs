using System;
using System.Collections.Generic;

namespace GameCore.Statistics;

public abstract class AStatusEffectDB
{
    protected AStatusEffectDB()
    {
        _effects = BuildDB();
    }

    private readonly StatusEffectData[] _effects;
    public IReadOnlyCollection<StatusEffectData> Effects => _effects;

    public StatusEffectData? GetEffectData(int type)
    {
        return Array.Find(_effects, effect => effect.EffectType.Equals(type));
    }

    protected abstract StatusEffectData[] BuildDB();
}
