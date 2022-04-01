using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.JumpStates
{
    public class Grounded : State<Actor>
    {
        public override void Enter()
        {
            Actor.VelocityY = Actor.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit() { }

        public override void CheckForTransitions()
        {
            if (InputHandler.Jump.IsActionJustPressed && !Actor.IsJumpDisabled)
                StateMachine.TransitionTo(new Jump());
            else if (!Actor.IsOnFloor())
                StateMachine.TransitionTo(new Fall());
        }
    }
}