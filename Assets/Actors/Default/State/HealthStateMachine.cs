using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Assets.Actors.Default.State
{
    public class HealthStateMachine : HealthStateMachineBase
    {
        public HealthStateMachine(Actor actor)
            : base(actor)
        {
            AddState<Normal>();
            AddState<Stagger>();
            AddState<Dead>();
            InitStates(this);
        }

        public class Normal : HealthState
        {
            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
            }

            public override HealthState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override HealthState CheckForTransitions()
            {
                return null;
            }

            public override void HandleDamage(DamageData damageData)
            {
                if (Actor.Stats.HasNoHP())
                    return;
                bool overDamageThreshold = damageData.TotalDamage > 0 && damageData.ActionType != ActionType.Status;
                Actor.IFrameController.Start(damageData, overDamageThreshold);
                if (overDamageThreshold)
                {
                    // Knockback
                    Vector2 direction = damageData.SourcePosition.DirectionTo(Actor.GlobalPosition);
                    Actor.Velocity = direction * 200;
                    StateMachine.TransitionTo<Stagger>(new[] { damageData });
                }
            }

            public override void HandleHPDepleted()
            {
                StateMachine.TransitionTo<Dead>();
            }
        }

        public class Stagger : HealthState
        {
            public Stagger()
            {
                AnimationName = "Stagger";
                BlockedStates = new BlockableState[]
                {
                    BlockableState.Attack,
                    BlockableState.Jumping,
                    BlockableState.Move
                };
            }

            float _staggerTimer;
            bool _isStaggered;

            public override void Enter(object[] args)
            {
                _staggerTimer = 0.5f;
                _isStaggered = true;
                Actor.PlaySoundFX("agh1.wav");
                PlayAnimation(AnimationName);
            }

            public override HealthState Update(float delta)
            {
                if (_staggerTimer > 0)
                    _staggerTimer -= delta;
                else
                    _isStaggered = false;
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override HealthState CheckForTransitions()
            {
                if (!_isStaggered)
                    return GetState<Normal>();
                return null;
            }

            public override void HandleHPDepleted()
            {
                StateMachine.TransitionTo<Dead>();
            }
        }

        public class Dead : HealthState
        {
            public Dead()
            {
                AnimationName = "Dead";
                BlockedStates = new BlockableState[]
                {
                    BlockableState.Attack,
                    BlockableState.Jumping,
                    BlockableState.Move
                };
            }

            public override void Enter()
            {
                Actor.Stats.AddKOStatus();
                Actor.IFrameController.Stop();
                Actor.HurtBoxes.SetMonitoringDeferred(false);
                Actor.Velocity = new Vector2(0, 0);
                PlayAnimation(AnimationName);
            }

            public override HealthState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit()
            {
                Actor.HurtBoxes.SetMonitoringDeferred(true);
            }

            public override HealthState CheckForTransitions()
            {
                if (!Actor.Stats.HasNoHP())
                    return GetState<Normal>();
                return null;
            }
        }
    }
}