using Arenbee.Assets.Actors.Enemies.Default.Animation;
using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Dead : HealthState
    {
        public override void Enter()
        {
            Actor.IFrameController.Stop();
            Actor.PlaySoundFX("agh2.wav");
            Actor.Velocity = new Vector2(0, 0);
            CreateDeathEffect();
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            return null;
        }

        private void CreateDeathEffect()
        {
            var enemyDeathEffectScene = GD.Load<PackedScene>(EnemyDeathEffect.GetScenePath());
            Node2D parent = Actor.GetParentOrNull<Node2D>();
            if (parent != null)
            {
                var enemyDeathEffect = enemyDeathEffectScene.Instantiate<EnemyDeathEffect>();
                enemyDeathEffect.Position = Actor.BodySprite.GlobalPosition;
                parent.AddChild(enemyDeathEffect);
                enemyDeathEffect.Play();
            }
        }
    }
}