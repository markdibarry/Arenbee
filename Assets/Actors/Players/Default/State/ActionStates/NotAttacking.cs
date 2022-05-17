using Arenbee.Assets.Actors.Default.State;
using Arenbee.Framework;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class NotAttacking : NotAttackingState
    {
        public override void Enter()
        {
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            var result = CheckForBaseTransitions(out bool returnEarly);
            if (returnEarly)
                return result;
            if (InputHandler.Attack.IsActionJustPressed)
                return StateMachine.GetState<UnarmedAttack>();
            return null;
        }
    }
}