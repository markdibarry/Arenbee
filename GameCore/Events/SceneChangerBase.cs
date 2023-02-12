﻿using GameCore.Actors;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace GameCore.Events;

public abstract partial class SceneChangerBase : Area2D
{
    protected TransitionControllerBase TController { get; } = Locator.TransitionController;
    protected GUIController GUIController { get; } = Locator.Root.GUIController;
    protected AGameSession? GameSession { get; } = Locator.Session;
    [Export]
    public bool Automatic { get; set; }
    [Export(PropertyHint.File)]
    public string PackedScenePath { get; set; } = string.Empty;
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
        if (body is not AActorBody actor || actor.ActorType != ActorType.Player)
            return;
        if (!FileAccess.FileExists(PackedScenePath))
            return;
        IsActive = true;
        ChangeScene();
    }

    /// <summary>
    /// Replaces an AreaScene with another
    /// </summary>
    protected abstract void ChangeScene();
}
