using System;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Actor
    {
        public BehaviorTree BehaviorTree { get; set; }
        public bool IsAttackDisabled { get; set; }
        public Sprite2D BodySprite { get; set; }
        public AnimationPlayer AnimationPlayer { get; set; }
        public StateController StateController { get; set; }
        private Blinker _blinker;
        public delegate void ActorDefeatedHandler(string actorName);
        public event ActorDefeatedHandler ActorDefeated;

        public virtual void OnHurtBoxEntered(Area2D area2D, HurtBox hurtBox)
        {
            ActorStats.HandleHitBoxAction((HitBox)area2D);
        }

        public virtual void OnHitBoxActionRecieved(HitBoxActionRecievedData data)
        {
            _blinker.Start(data.TotalDamage > 0);
            if (data.TotalDamage > 0) HandleKnockBack(data.SourcePosition);

            if (data.TotalDamage > 0)
            {
                if (ActorType == ActorType.Player)
                {
                    StateController.BaseStateMachine.TransitionTo(new Assets.Players.MoveStates.Stagger());
                }
                else if (ActorType == ActorType.Enemy)
                {
                    StateController.BaseStateMachine.TransitionTo(new Assets.Enemies.MoveStates.Stagger());
                }
            }
        }

        public void HandleKnockBack(Vector2 hitPosition)
        {
            Vector2 direction = hitPosition.DirectionTo(GlobalPosition);
            MotionVelocity = direction * -JumpVelocity;
        }

        private void OnHPDepleted(object sender, EventArgs e)
        {
            _blinker.Stop();
            HurtBox.SetDeferred("monitoring", false);
            HandleHPDepleted();
        }

        protected virtual void HandleHPDepleted()
        {
            ActorDefeated?.Invoke(Name);
            if (ActorType == ActorType.Player)
            {
                StateController.BaseStateMachine.TransitionTo(new Assets.Players.MoveStates.Dead());
            }
            else if (ActorType == ActorType.Enemy)
            {
                StateController.BaseStateMachine.TransitionTo(new Assets.Enemies.MoveStates.Dead());
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
