using System.Collections.Generic;

namespace GameCore.ActionEffects;

public abstract class ActionEffectDBBase
{
    protected Dictionary<ActionEffectType, IActionEffect> Effects { get; } = new();

    public IActionEffect GetEffect(ActionEffectType type)
    {
        if (Effects.TryGetValue(type, out var effect))
            return effect;
        return null;
    }
}
