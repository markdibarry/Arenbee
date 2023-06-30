using Arenbee.Statistics;
using GameCore.Actors;
using Godot;

namespace Arenbee.Actors.State.DefaultPlayer;

public class HealthStateMachine : HealthStateMachineBase
{
    public HealthStateMachine(ActorBody actorBody)
        : base(
            new HealthState[]
            {
                new Normal(actorBody),
                new Stagger(actorBody),
                new Dead(actorBody)
            },
            actorBody)
    {
    }

    public class Normal : HealthState
    {
        public Normal(ActorBody actorBody) : base(actorBody)
        {
        }

        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Stats == null)
                return false;
            if (Stats.HasNoHP)
                return stateMachine.TrySwitchTo<Dead>();
            if (Stats.CurrentDamageResult != null)
            {
                DamageResult damageResult = (DamageResult)Stats.CurrentDamageResult;
                bool overDamageThreshold = damageResult.TotalDamage > 0 && damageResult.ActionType != ActionType.Status;
                ActorBody.IFrameController.Start(damageResult, overDamageThreshold);
                if (overDamageThreshold)
                {
                    // Knockback
                    Vector2 direction = damageResult.SourcePosition.DirectionTo(ActorBody.GlobalPosition);
                    ActorBody.Velocity = direction * 200;
                    stateMachine.TrySwitchTo<Stagger>();
                }
            }
            return false;
        }
    }

    public class Stagger : HealthState
    {
        public Stagger(ActorBody actorBody)
            : base(actorBody)
        {
            AnimationName = "Stagger";
            BlockedStates =
                BlockedState.Attack |
                BlockedState.Jumping |
                BlockedState.Move;
        }

        double _staggerTimer;
        bool _isStaggered;

        public override void Enter()
        {
            _staggerTimer = 0.5;
            _isStaggered = true;
            ActorBody.PlaySoundFX("agh1.wav");
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
            if (_staggerTimer > 0)
                _staggerTimer -= delta;
            else
                _isStaggered = false;
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Stats != null && Stats.HasNoHP)
                return stateMachine.TrySwitchTo<Dead>();
            if (!_isStaggered)
                return stateMachine.TrySwitchTo<Normal>();
            return false;
        }
    }

    public class Dead : HealthState
    {
        public Dead(ActorBody actorBody) : base(actorBody)
        {
            AnimationName = "Dead";
            BlockedStates =
                BlockedState.Attack |
                BlockedState.Jumping |
                BlockedState.Move;
        }

        public override void Enter()
        {
            Stats?.AddKOStatus();
            ActorBody.IFrameController.Stop();
            ActorBody.HurtBoxes.SetMonitoringDeferred(false);
            ActorBody.Velocity = new Vector2(0, 0);
            PlayAnimation(AnimationName);
        }

        public override void Exit()
        {
            ActorBody.HurtBoxes.SetMonitoringDeferred(true);
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Stats != null && !Stats.HasNoHP)
                return stateMachine.TrySwitchTo<Normal>();
            return false;
        }
    }
}
