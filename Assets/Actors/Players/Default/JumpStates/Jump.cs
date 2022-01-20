using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.JumpStates
{
    public class Jump : State<Actor>
    {
        public Jump() { AnimationName = "Jump"; }
        public override void Enter()
        {
            Actor.Jump();
            StateMachine.PlayAnimation(AnimationName);
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