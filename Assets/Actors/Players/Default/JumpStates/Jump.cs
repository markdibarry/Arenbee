using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.JumpStates
{
    public class Jump : State<Actor>
    {
        public override void Enter()
        {
            AnimationName = "Jump";
            Actor.Jump();
            StateController.PlayBase(AnimationName);
        }

        public override void Update(float delta)
        {
            if (InputHandler.Jump.IsActionJustReleased)
                Actor.MotionVelocityY = Actor.MotionVelocity.y * 0.5f;
            Actor.MotionVelocityY = Actor.MotionVelocity.y + (Actor.JumpGravity * delta);

            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (Actor.IsOnFloor())
            {
                StateMachine.TransitionTo(new Grounded());
            }
            else if (Actor.MotionVelocity.y >= 0)
            {
                StateMachine.TransitionTo(new Fall());
            }
        }
    }
}