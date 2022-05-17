using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class Grounded : JumpState
    {
        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (!Actor.IsOnFloor())
                return GetState<Falling>();
            if (InputHandler.Jump.IsActionJustPressed && !StateController.IsBlocked(ActorStateType.Jump))
                return GetState<Jump>();
            return null;
        }
    }
}