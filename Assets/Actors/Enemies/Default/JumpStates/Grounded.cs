using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.JumpStates
{
    public class Grounded : State<Actor>
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
            if (!Actor.IsOnFloor())
            {
                StateMachine.TransitionTo(new Fall());
            }
        }
    }
}