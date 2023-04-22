using System.Collections.Generic;
using GameCore.ActionEffects;

namespace Arenbee.ActionEffects;

public class ActionEffectDB : AActionEffectDB
{
    protected override IReadOnlyDictionary<int, IActionEffect> BuildDB()
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
