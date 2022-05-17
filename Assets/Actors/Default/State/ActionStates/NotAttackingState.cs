using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Default.State
{
    public abstract class NotAttackingState : ActionState
    {
        public override ActorState CheckForBaseTransitions(out bool returnEarly)
        {
            returnEarly = false;
            if (StateController.IsBlocked(ActorStateType.Attack) || Actor.ContextAreasActive > 0)
                returnEarly = true;
            return null;
        }
    }
}