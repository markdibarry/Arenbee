using Godot;
using System;

public partial class EnemyDeathEffect : AnimatedSprite2D
{
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
