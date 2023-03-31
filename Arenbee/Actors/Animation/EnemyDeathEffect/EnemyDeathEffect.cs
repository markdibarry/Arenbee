using GameCore.Extensions;
using Godot;

namespace Arenbee.Actors.Animation;

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
