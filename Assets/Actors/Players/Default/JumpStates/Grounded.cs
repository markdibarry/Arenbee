using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.JumpStates
{
    public class Grounded : State<Player>
    {
        public override void Enter()
        {
            Actor.MotionVelocityY = Actor.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (StateMachine.IsActionJustPressed(Actor.InputHandler.Jump))
            {
                StateMachine.TransitionTo(new Jump());
            }
            else if (!Actor.IsOnFloor())
            {
                StateMachine.TransitionTo(new Fall());
            }
        }
    }
}