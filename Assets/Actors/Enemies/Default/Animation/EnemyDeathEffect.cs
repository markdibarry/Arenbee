using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.Actors.Enemies.ActionStates
{
    public partial class EnemyDeathEffect : AnimatedSprite2D
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public override void _Ready()
        {
            AnimationFinished += OnAnimationFinished;
        }

        private void OnAnimationFinished()
        {
            AnimationFinished -= OnAnimationFinished;
            QueueFree();
        }
    }
}