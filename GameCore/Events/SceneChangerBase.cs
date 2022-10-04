using GameCore.Actors;
using Godot;

namespace GameCore.Events;

public abstract partial class SceneChangerBase : Area2D
{
    [Export]
    public bool Automatic { get; set; }
    [Export(PropertyHint.File)]
    public string PackedScenePath { get; set; }
    public bool IsActive { get; set; }

    public override void _Ready()
    {
        if (Automatic)
            BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node body)
    {
        if (IsActive || PackedScenePath == null)
            return;
        if (body is not ActorBase actor || actor.ActorType != ActorType.Player)
            return;
        if (!FileAccess.FileExists(PackedScenePath))
            return;
        IsActive = true;
        ChangeScene();
    }

    protected abstract void ChangeScene();
}
