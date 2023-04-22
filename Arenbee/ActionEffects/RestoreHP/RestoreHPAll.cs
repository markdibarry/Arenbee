using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Utility;

namespace Arenbee.ActionEffects;

public class RestoreHPAll : IActionEffect
{
    public bool IsActionSequence => true;
    public int TargetType => (int)ActionEffects.TargetType.PartyMemberAll;
    private static readonly IActionEffect _restoreHP = Locator.ActionEffectDB.GetEffect((int)ActionEffectType.RestoreHP)!;

    public bool CanUse(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        return targets.Any(x => _restoreHP.CanUse(null, new[] { x }, actionType, value1, value2));
    }

    public async Task Use(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        List<Task> tasks = new();
        foreach (AActor target in targets)
            tasks.Add(_restoreHP.Use(user, new[] { target }, actionType, value1, value2));
        await Task.WhenAll(tasks);
    }
}
