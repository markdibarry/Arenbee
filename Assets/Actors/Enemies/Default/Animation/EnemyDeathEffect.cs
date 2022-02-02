using Godot;

public partial class EnemyDeathEffect : AnimatedSprite2D
{
    public static readonly string ScenePath = $"res://Assets/Actors/Enemies/Default/Animation/{nameof(EnemyDeathEffect)}.tscn";
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
