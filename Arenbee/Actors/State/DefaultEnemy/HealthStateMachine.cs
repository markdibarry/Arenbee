using Arenbee.Actors.Animation;
using Arenbee.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Actors.State.DefaultEnemy;

public class HealthStateMachine : HealthStateMachineBase
{
    public HealthStateMachine(ActorBody actor)
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
        public Normal(ActorBody actor) : base(actor)
        {
        }

        public override void Enter() { }

        public override void Update(double delta) { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
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
        public Stagger(ActorBody actor)
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
            if (Stats.HasNoHP)
                return stateMachine.TrySwitchTo<Dead>();
            if (!_isStaggered)
                return stateMachine.TrySwitchTo<Normal>();
            return false;
        }
    }

    public class Dead : HealthState
    {
        public Dead(ActorBody actor) : base(actor)
        {
        }

        public override void Enter()
        {
            ActorBody.IFrameController.Stop();
            ActorBody.PlaySoundFX("agh2.wav");
            ActorBody.Velocity = new Vector2(0, 0);
            CreateDeathEffect();
            ActorBody.QueueFree();
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
