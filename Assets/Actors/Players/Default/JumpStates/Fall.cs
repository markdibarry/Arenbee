using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Players.JumpStates
{
    public class Fall : State<Actor>
    {
        readonly float _fallMultiplier = 2f;
        float _jumpGraceTimer = 0;
        readonly float _jumpGraceTime = 0.1f;
        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            Actor.VelocityY = Actor.Velocity.y.LerpClamp(Actor.JumpGravity * _fallMultiplier, Actor.JumpGravity * delta);
            if (_jumpGraceTimer > 0)
                _jumpGraceTimer -= delta;

            if (InputHandler.Jump.IsActionJustPressed)
                _jumpGraceTimer = _jumpGraceTime;
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (Actor.IsOnFloor())
            {
                if (_jumpGraceTimer > 0 && !Actor.IsJumpDisabled)
                    StateMachine.TransitionTo(new Jump());
                else
                    StateMachine.TransitionTo(new Grounded());
            }
        }
    }
}