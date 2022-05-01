using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Players.JumpStates
{
    public class Fall : ActorState
    {
        readonly float _fallMultiplier = 2f;
        float _jumpGraceTimer = 0;
        readonly float _jumpGraceTime = 0.1f;

        public Fall() { AnimationName = "Jump"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            Actor.VelocityY = Actor.Velocity.y.LerpClamp(Actor.JumpGravity * _fallMultiplier, Actor.JumpGravity * delta);
            if (_jumpGraceTimer > 0)
                _jumpGraceTimer -= delta;

            if (InputHandler.Jump.IsActionJustPressed)
                _jumpGraceTimer = _jumpGraceTime;
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsOnFloor())
            {
                if (_jumpGraceTimer > 0 && !Actor.IsJumpDisabled)
                    return new Jump();
                return new Grounded();
            }
            return null;
        }
    }
}