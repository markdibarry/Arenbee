using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee.ActionEffects;

public class RestoreHP : IActionEffect
{
    public bool CanUse(ActionEffectRequest request, AActor[] targets)
    {
        if (targets.Length == 0)
            return false;
        Stats stats = (Stats)targets[0].Stats;
        return !stats.HasFullHP && !stats.HasNoHP;
    }

    public void Use(ActionEffectRequest request, AActor[] targets)
    {
        AActor target = targets[0];
        var actionData = new DamageRequest()
        {
            SourceName = target.Name,
            ActionType = request.ActionType,
            Value = request.Value1 * -1,
            ElementType = ElementType.Healing
        };

        target.Stats.ReceiveDamageRequest(actionData);
    }
}
