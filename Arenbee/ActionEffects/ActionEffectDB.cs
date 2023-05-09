using System.Collections.Generic;
using GameCore.ActionEffects;

namespace Arenbee.ActionEffects;

public class ActionEffectDB : IActionEffectDB
{
    protected IReadOnlyDictionary<int, IActionEffect> Effects { get; } = BuildDB();

    public IActionEffect? GetEffect(int type)
    {
        if (Effects.TryGetValue(type, out IActionEffect? effect))
            return effect;
        return null;
    }

    private static IReadOnlyDictionary<int, IActionEffect> BuildDB()
    {
        Dictionary<int, IActionEffect> effects = new()
        {
            [(int)ActionEffectType.RestoreHP] = new RestoreHP(),
            [(int)ActionEffectType.RestoreHPAll] = new RestoreHPAll(),
            [(int)ActionEffectType.CurePoison] = new CurePoison(),
            [(int)ActionEffectType.Darts] = new Darts()
        };
        return effects;
    }
}
