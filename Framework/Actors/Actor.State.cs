using Arenbee.Assets.Actors.Default.BaseStates;
using Arenbee.Assets.Actors.Enemies.ActionStates;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        public bool IsAttackDisabled { get; set; }
        public bool IsWalkDisabled { get; set; }
        public bool IsRunDisabled { get; set; }
        public bool IsJumpDisabled { get; set; }
        public Sprite2D BodySprite { get; private set; }
        public AnimationPlayer AnimationPlayer { get; private set; }
        public StateController StateController { get; private set; }
        protected BehaviorTree BehaviorTree { get; set; }
        private Blinker _blinker;
        private PackedScene _enemyDeathEffectScene;
        public delegate void ActorDefeatedHandler(Actor actor);
        public delegate void ActorRemovedHandler(Actor actor);
        public delegate void DamageRecievedHandler(DamageData damageRecievedData);
        public event ActorDefeatedHandler ActorDefeated;
        public event ActorRemovedHandler ActorRemoved;
        public event DamageRecievedHandler DamageRecieved;

        public void CreateDeathEffect()
        {
            Node2D parent = GetParentOrNull<Node2D>();
            if (parent != null)
            {
                var enemyDeathEffect = _enemyDeathEffectScene.Instantiate<EnemyDeathEffect>();
                enemyDeathEffect.Position = BodySprite.GlobalPosition;
                parent.AddChild(enemyDeathEffect);
                enemyDeathEffect.Play();
            }
        }

        private void InitState()
        {
            _blinker.Init(this);
            StateController = new StateController(this);
            _enemyDeathEffectScene = GD.Load<PackedScene>(EnemyDeathEffect.GetScenePath());
        }

        private void HandleDamage(int damage, Vector2 sourcePosition)
        {
            _blinker.Start(damage > 0);
            if (damage > 0)
            {
                HandleKnockBack(sourcePosition);
                StateController.BaseStateMachine.TransitionTo(new Stagger());
            }
        }

        private void HandleHPDepleted()
        {
            _blinker.Stop();
            HurtBox.SetDeferred("monitoring", false);
            ActorDefeated?.Invoke(this);
            if (ActorType == ActorType.Player)
            {
                StateController.BaseStateMachine.TransitionTo(new Assets.Actors.Players.BaseStates.Dead());
            }
            else if (ActorType == ActorType.Enemy)
            {
                StateController.BaseStateMachine.TransitionTo(new Assets.Actors.Enemies.BaseStates.Dead());
            }
        }

        private void HandleKnockBack(Vector2 hitPosition)
        {
            Vector2 direction = hitPosition.DirectionTo(GlobalPosition);
            Velocity = direction * -JumpVelocity;
        }

        private void OnDamageRecieved(DamageData data)
        {
            data.RecieverName = Name;
            HandleDamage(data.TotalDamage, data.SourcePosition);
            DamageRecieved?.Invoke(data);
        }

        private void OnHPDepleted()
        {
            HandleHPDepleted();
        }
    }
}
