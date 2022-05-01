using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.JumpStates
{
    public class Grounded : ActorState
    {
        public override void Enter()
        {
            Actor.VelocityY = Actor.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (InputHandler.Jump.IsActionJustPressed && !Actor.IsJumpDisabled)
                return new Jump();
            else if (!Actor.IsOnFloor())
                return new Fall();
            return null;
        }
    }
}