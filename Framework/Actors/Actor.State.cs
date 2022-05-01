using Arenbee.Assets.Actors.Default.BaseStates;
using Arenbee.Assets.Actors.Enemies.ActionStates;
using Arenbee.Framework.Statistics;
using Godot;
using Arenbee.Framework.Enums;

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

        private void HandleDamage(DamageData damageData)
        {
            damageData.RecieverName = Name;
            _blinker.Start(damageData);
            if (damageData.TotalDamage > 0 && damageData.ActionType != ActionType.Status)
            {
                HandleKnockBack(damageData.SourcePosition);
                StateController.BaseStateMachine.TransitionTo(new Stagger());
            }
            DamageRecieved?.Invoke(damageData);
        }

        private void HandleHPDepleted()
        {
            if (!_isReady) return;
            _blinker.Stop();
            HurtBox.SetDeferred("monitoring", false);
            ActorDefeated?.Invoke(this);
            if (ActorType == ActorType.Player)
                StateController.BaseStateMachine.TransitionTo(new Assets.Actors.Players.BaseStates.Dead());
            else if (ActorType == ActorType.Enemy)
                StateController.BaseStateMachine.TransitionTo(new Assets.Actors.Enemies.BaseStates.Dead());
        }

        private void HandleKnockBack(Vector2 hitPosition)
        {
            Vector2 direction = hitPosition.DirectionTo(GlobalPosition);
            Velocity = direction * (int)(JumpVelocity * -0.5);
        }

        private void OnDamageRecieved(DamageData data)
        {
            HandleDamage(data);
        }

        private void OnHPDepleted()
        {
            HandleHPDepleted();
        }
    }
}
