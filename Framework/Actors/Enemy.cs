using Arenbee.Framework.Constants;
using Arenbee.Assets.Enemies.MoveStates;
using Godot;
using Arenbee.Framework.Actors.Stats;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Enemy : Actor
    {
        public delegate void EnemyDefeatedHandler(string enemyName);
        public event EnemyDefeatedHandler EnemyDefeated;
        private PackedScene _enemyDeathEffectScene;

        public override void SetDefaults()
        {
            base.SetDefaults();
            WalkSpeed = 50;
        }

        public override void _Ready()
        {
            base._Ready();
            _enemyDeathEffectScene = GD.Load<PackedScene>(PathConstants.EnemyDeathEffect);
        }

        public override void OnHitBoxActionRecieved(HitBoxActionRecievedData data)
        {
            base.OnHitBoxActionRecieved(data);

            if (data.TotalDamage > 0)
                StateController.BaseStateMachine.TransitionTo(new Stagger());
        }

        protected override void HandleHPDepleted()
        {
            EnemyDefeated?.Invoke(Name);
            StateController.BaseStateMachine.TransitionTo(new Dead());
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
