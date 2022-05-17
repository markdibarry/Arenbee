using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class Jump : JumpState
    {
        public Jump() { AnimationName = "Jump"; }

        private bool _isJumpReleased;

        public override void Enter()
        {
            _isJumpReleased = false;
            //Actor.PlaySoundFX("no1.wav");
            Actor.Jump();
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            if (!_isJumpReleased && !InputHandler.Jump.IsActionPressed)
            {
                Actor.VelocityY = Actor.Velocity.y * 0.5f;
                _isJumpReleased = true;
            }

            Actor.ApplyJumpGravity(delta);
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsMovingDown() || StateController.IsBlocked(ActorStateType.Jump))
                return GetState<Falling>();
            if (Actor.IsOnFloor())
                return GetState<Grounded>();
            return null;
        }
    }
}