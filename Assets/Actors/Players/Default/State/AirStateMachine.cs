using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Default.State
{
    public class AirStateMachine : AirStateMachineBase
    {
        public AirStateMachine(Actor actor)
            : base(actor)
        {
            AddState<Grounded>();
            AddState<Jumping>();
            AddState<Falling>();
            InitStates(this);
        }

        public class Grounded : AirState
        {
            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
            }

            public override AirState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override AirState CheckForTransitions()
            {
                if (!Actor.IsOnFloor())
                    return GetState<Falling>();
                if (InputHandler.Jump.IsActionJustPressed && !StateController.IsBlocked(BlockableState.Jumping))
                    return GetState<Jumping>();
                return null;
            }
        }

        public class Jumping : AirState
        {
            public Jumping() { AnimationName = "Jump"; }

            private bool _isJumpReleased;

            public override void Enter()
            {
                _isJumpReleased = false;
                //Actor.PlaySoundFX("no1.wav");
                Actor.Jump();
                PlayAnimation(AnimationName);
            }

            public override AirState Update(float delta)
            {
                if (!_isJumpReleased && !InputHandler.Jump.IsActionPressed)
                {
                    Actor.VelocityY = Actor.Velocity.y * 0.5f;
                    _isJumpReleased = true;
                }

                Actor.ApplyJumpGravity(delta);
                return CheckForTransitions();
            }

            public override AirState CheckForTransitions()
            {
                if (Actor.IsMovingDown() || StateController.IsBlocked(BlockableState.Jumping))
                    return GetState<Falling>();
                if (Actor.IsOnFloor())
                    return GetState<Grounded>();
                return null;
            }
        }

        public class Falling : AirState
        {
            float _jumpGraceTimer;
            readonly float _jumpGraceTime = 0.1f;

            public Falling() { AnimationName = "Jump"; }

            public override void Enter()
            {
                _jumpGraceTimer = 0;
                PlayAnimation(AnimationName);
            }

            public override AirState Update(float delta)
            {
                Actor.ApplyFallGravity(delta);
                if (_jumpGraceTimer > 0)
                    _jumpGraceTimer -= delta;
                if (InputHandler.Jump.IsActionJustPressed)
                    _jumpGraceTimer = _jumpGraceTime;
                return CheckForTransitions();
            }

            public override AirState CheckForTransitions()
            {
                if (Actor.IsOnFloor())
                {
                    if (_jumpGraceTimer > 0 && !StateController.IsBlocked(BlockableState.Jumping))
                        return GetState<Jumping>();
                    return GetState<Grounded>();
                }
                return null;
            }
        }
    }
}