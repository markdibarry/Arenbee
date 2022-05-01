using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.JumpStates
{
    public class Jump : ActorState
    {
        public Jump() { AnimationName = "Jump"; }

        public override void Enter()
        {
            Actor.Jump();
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            if (InputHandler.Jump.IsActionJustReleased)
                Actor.VelocityY = Actor.Velocity.y * 0.5f;
            Actor.VelocityY = Actor.Velocity.y + (Actor.JumpGravity * delta);

            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsOnFloor())
                return new Grounded();
            else if (Actor.Velocity.y >= 0)
                return new Fall();
            return null;
        }
    }
}