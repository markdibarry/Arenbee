using GameCore.Actors;
using GameCore.Enums;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Actors.Default.State;

public class HealthStateMachine : HealthStateMachineBase
{
    public HealthStateMachine(ActorBase actor)
        : base(
            new HealthState[]
            {
                new Normal(actor),
                new Stagger(actor),
                new Dead(actor)
            },
            actor)
    {
    }

    public class Normal : HealthState
    {
        public Normal(ActorBase actor) : base(actor)
        {
        }

        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override void Update(double delta) { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Actor.Stats.HasNoHP())
                return stateMachine.TrySwitchTo<Dead>();
            if (Actor.Stats.DamageToProcess.Count > 0)
            {
                DamageData damageData = Actor.Stats.DamageToProcess[0];
                bool overDamageThreshold = damageData.TotalDamage > 0 && damageData.ActionType != ActionType.Status;
                Actor.IFrameController.Start(damageData, overDamageThreshold);
                if (overDamageThreshold)
                {
                    // Knockback
                    Vector2 direction = damageData.SourcePosition.DirectionTo(Actor.GlobalPosition);
                    Actor.Velocity = direction * 200;
                    stateMachine.TrySwitchTo<Stagger>();
                }
            }
            return false;
        }
    }

    public class Stagger : HealthState
    {
        public Stagger(ActorBase actor)
            : base(actor)
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
            Actor.PlaySoundFX("agh1.wav");
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
            if (_staggerTimer > 0)
                _staggerTimer -= delta;
            else
                _isStaggered = false;
        }

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Actor.Stats.HasNoHP())
                return stateMachine.TrySwitchTo<Dead>();
            if (!_isStaggered)
                return stateMachine.TrySwitchTo<Normal>();
            return false;
        }
    }

    public class Dead : HealthState
    {
        public Dead(ActorBase actor) : base(actor)
        {
            AnimationName = "Dead";
            BlockedStates =
                BlockedState.Attack |
                BlockedState.Jumping |
                BlockedState.Move;
        }

        public override void Enter()
        {
            Actor.Stats.AddKOStatus();
            Actor.IFrameController.Stop();
            Actor.HurtBoxes.SetMonitoringDeferred(false);
            Actor.Velocity = new Vector2(0, 0);
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
        }

        public override void Exit()
        {
            Actor.HurtBoxes.SetMonitoringDeferred(true);
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (!Actor.Stats.HasNoHP())
                return stateMachine.TrySwitchTo<Normal>();
            return false;
        }
    }
}
