using Arenbee.Framework.Enums;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Input;
using Godot;
using Arenbee.Assets.Actors.Default.BaseStates;
using Arenbee.Framework.Constants;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Actor
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
        public delegate void ActorDefeatedHandler(string actorName);
        public event ActorDefeatedHandler ActorDefeated;

        private void InitState()
        {
            _blinker.Init(this);
            HurtBox.AreaEntered += (area2d) => OnHurtBoxEntered(area2d);
            StateController = new StateController(this);
            WeaponSlot.Init(this, _equipment.GetSlot(EquipmentSlotName.Weapon));
            _enemyDeathEffectScene = GD.Load<PackedScene>(PathConstants.EnemyDeathEffect);
        }

        private void OnHurtBoxEntered(Area2D area2D)
        {
            HandleHitBoxAction((HitBox)area2D);
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

        private void HandleKnockBack(Vector2 hitPosition)
        {
            Vector2 direction = hitPosition.DirectionTo(GlobalPosition);
            MotionVelocity = direction * -JumpVelocity;
        }

        private void HandleHPDepleted()
        {
            _blinker.Stop();
            HurtBox.SetDeferred("monitoring", false);
            ActorDefeated?.Invoke(Name);
            if (ActorType == ActorType.Player)
            {
                StateController.BaseStateMachine.TransitionTo(new Assets.Actors.Players.BaseStates.Dead());
            }
            else if (ActorType == ActorType.Enemy)
            {
                StateController.BaseStateMachine.TransitionTo(new Assets.Actors.Enemies.BaseStates.Dead());
            }
        }

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
    }
}
