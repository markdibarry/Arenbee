using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee.ActionEffects
{
    public class RestoreHP : IActionEffect
    {
        public bool CanUse(ActionEffectRequest request, Actor[] targets)
        {
            if (targets == null || targets.Length == 0)
                return false;
            Actor target = targets[0];
            return !target.Stats.HasFullHP() && !target.Stats.HasNoHP();
        }

        public void Use(ActionEffectRequest request, Actor[] targets)
        {
            Actor target = targets[0];
            var actionData = new ActionData()
            {
                SourceName = target.Name,
                ActionType = request.ActionType,
                Value = request.Value1 * -1,
                ElementDamage = ElementType.Healing
            };

            targets[0].Stats.ReceiveAction(actionData);
        }
    }
}