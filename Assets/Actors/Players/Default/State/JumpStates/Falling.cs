using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class Falling : JumpState
    {
        float _jumpGraceTimer;
        readonly float _jumpGraceTime = 0.1f;

        public Falling() { AnimationName = "Jump"; }

        public override void Enter()
        {
            _jumpGraceTimer = 0;
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            Actor.ApplyFallGravity(delta);
            if (_jumpGraceTimer > 0)
                _jumpGraceTimer -= delta;
            if (InputHandler.Jump.IsActionJustPressed)
                _jumpGraceTimer = _jumpGraceTime;
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsOnFloor())
            {
                if (_jumpGraceTimer > 0 && !StateController.IsBlocked(ActorStateType.Jump))
                    return GetState<Jump>();
                return GetState<Grounded>();
            }
            return null;
        }
    }
}