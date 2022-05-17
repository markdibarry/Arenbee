using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Default.State
{
    public abstract class AttackingState : ActionState
    {
        public override ActorState CheckForBaseTransitions(out bool returnEarly)
        {
            returnEarly = false;
            if (StateController.IsBlocked(ActorStateType.Attack))
            {
                returnEarly = true;
                return StateMachine.FallbackState;
            }

            return null;
        }
    }
}