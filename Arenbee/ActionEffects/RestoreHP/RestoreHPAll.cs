using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore;
using GameCore.ActionEffects;
using GameCore.Actors;

namespace Arenbee.ActionEffects;

public class RestoreHPAll : IActionEffect
{
    public bool IsActionSequence => true;
    public int TargetType => (int)ActionEffects.TargetType.PartyMemberAll;

    public bool CanUse(BaseActor? user, IList<BaseActor> targets, int actionType, int value1, int value2)
    {
        IActionEffect? restoreHP = Locator.ActionEffectDB.GetEffect((int)ActionEffectType.RestoreHP);
        if (restoreHP == null)
            return false;
        return targets.Any(x => restoreHP.CanUse(null, new[] { x }, actionType, value1, value2));
    }

    public async Task Use(BaseActor? user, IList<BaseActor> targets, int actionType, int value1, int value2)
    {
        IActionEffect? restoreHP = Locator.ActionEffectDB.GetEffect((int)ActionEffectType.RestoreHP);
        if (restoreHP == null)
            return;
        List<Task> tasks = new();
        foreach (BaseActor target in targets)
            tasks.Add(restoreHP.Use(user, new[] { target }, actionType, value1, value2));
        await Task.WhenAll(tasks);
    }
}
