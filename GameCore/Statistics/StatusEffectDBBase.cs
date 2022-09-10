using System.Collections.Generic;

namespace GameCore.Statistics;

public abstract class StatusEffectDBBase
{
    protected Dictionary<StatusEffectType, StatusEffectData> Effects { get; } = new();

    public StatusEffectData GetEffectData(StatusEffectType type)
    {
        if (Effects.TryGetValue(type, out var effect))
            return effect;
        return null;
    }

    public StatusEffectData GetEffectData(int type)
    {
        return GetEffectData((StatusEffectType)type);
    }

}
