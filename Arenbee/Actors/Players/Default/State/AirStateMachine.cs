using GameCore.Actors;
using GameCore.Utility;

namespace Arenbee.Actors.Default.State;

public class AirStateMachine : AirStateMachineBase
{
    public AirStateMachine(ActorBase actor)
        : base(
            new AirState[]
            {
                new Grounded(actor),
                new Jumping(actor),
                new Falling(actor)
            },
            actor)
    {
    }

    public class Grounded : AirState
    {
        public Grounded(ActorBase actor) : base(actor)
        {
        }

        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override void Update(double delta)
        {
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (!Actor.IsOnFloor())
                return stateMachine.TrySwitchTo<Falling>();
            if (InputHandler.Jump.IsActionJustPressed && !StateController.IsBlocked(BlockedState.Jumping))
                return stateMachine.TrySwitchTo<Jumping>();
            return false;
        }
    }

    public class Jumping : AirState
    {
        public Jumping(ActorBase actor) : base(actor)
        {
            AnimationName = "Jump";
        }

        private bool _isJumpReleased;

        public override void Enter()
        {
            _isJumpReleased = false;
            //Actor.PlaySoundFX("no1.wav");
            Actor.Jump();
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
            if (!_isJumpReleased && !InputHandler.Jump.IsActionPressed)
            {
                Actor.VelocityY = Actor.Velocity.y * 0.5f;
                _isJumpReleased = true;
            }

            Actor.ApplyJumpGravity(delta);
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Actor.IsMovingDown() || StateController.IsBlocked(BlockedState.Jumping))
                return stateMachine.TrySwitchTo<Falling>();
            if (Actor.IsOnFloor())
                return stateMachine.TrySwitchTo<Grounded>();
            return false;
        }
    }

    public class Falling : AirState
    {
        double _jumpGraceTimer;
        readonly double _jumpGraceTime = 0.1;

        public Falling(ActorBase actor) : base(actor)
        {
            AnimationName = "Jump";
        }

        public override void Enter()
        {
            _jumpGraceTimer = 0;
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
            Actor.ApplyFallGravity(delta);
            if (_jumpGraceTimer > 0)
                _jumpGraceTimer -= delta;
            if (InputHandler.Jump.IsActionJustPressed)
                _jumpGraceTimer = _jumpGraceTime;
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Actor.IsOnFloor())
            {
                if (_jumpGraceTimer > 0 && !StateController.IsBlocked(BlockedState.Jumping))
                    return stateMachine.TrySwitchTo<Jumping>();
                return stateMachine.TrySwitchTo<Grounded>();
            }
            return false;
        }
    }
}
