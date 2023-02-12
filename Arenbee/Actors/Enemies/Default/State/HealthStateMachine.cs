using Arenbee.Actors.Enemies.Default.Animation;
using GameCore.Actors;
using GameCore.Enums;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Actors.Enemies.Default.State;

public class HealthStateMachine : HealthStateMachineBase
{
    public HealthStateMachine(AActorBody actor)
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
        public Normal(AActorBody actor) : base(actor)
        {
        }

        public override void Enter() { }

        public override void Update(double delta) { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Stats.HasNoHP())
                return stateMachine.TrySwitchTo<Dead>();
            if (Stats.DamageToProcess.Count > 0)
            {
                DamageData damageData = Stats.DamageToProcess[0];
                bool overDamageThreshold = damageData.TotalDamage > 0 && damageData.ActionType != ActionType.Status;
                ActorBody.IFrameController.Start(damageData, overDamageThreshold);
                if (overDamageThreshold)
                {
                    // Knockback
                    Vector2 direction = damageData.SourcePosition.DirectionTo(ActorBody.GlobalPosition);
                    ActorBody.Velocity = direction * 200;
                    stateMachine.TrySwitchTo<Stagger>();
                }
            }
            return false;
        }
    }

    public class Stagger : HealthState
    {
        public Stagger(AActorBody actor)
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
            _staggerTimer = 1;
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

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (Stats.HasNoHP())
                return stateMachine.TrySwitchTo<Dead>();
            if (!_isStaggered)
                return stateMachine.TrySwitchTo<Normal>();
            return false;
        }
    }

    public class Dead : HealthState
    {
        public Dead(AActorBody actor) : base(actor)
        {
        }

        public override void Enter()
        {
            ActorBody.IFrameController.Stop();
            ActorBody.PlaySoundFX("agh2.wav");
            ActorBody.Velocity = new Vector2(0, 0);
            CreateDeathEffect();
        }

        public override void Update(double delta) { }

        private void CreateDeathEffect()
        {
            var enemyDeathEffectScene = GD.Load<PackedScene>(EnemyDeathEffect.GetScenePath());
            Node2D parent = ActorBody.GetParentOrNull<Node2D>();
            if (parent != null)
            {
                var enemyDeathEffect = enemyDeathEffectScene.Instantiate<EnemyDeathEffect>();
                enemyDeathEffect.Position = ActorBody.BodySprite.GlobalPosition;
                parent.AddChild(enemyDeathEffect);
                enemyDeathEffect.Play();
            }
        }
    }
}
