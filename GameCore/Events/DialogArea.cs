using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace GameCore.Events;

public partial class DialogArea : Area2D
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private ActorBase _actor;
    private bool _canTrigger;
    private ColorRect _colorRect;
    [Export]
    public string DialogPath { get; set; }
    [Export]
    public bool IsActive { get; set; } = true;
    [Export]
    public bool Hint
    {
        get => _colorRect?.Visible ?? false;
        set
        {
            if (_colorRect != null)
                _colorRect.Visible = value;
        }
    }

    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("ColorRect");
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public void OnBodyEntered(Node body)
    {
        if (_actor != null
            || body is not ActorBase actor
            || actor.ActorType != ActorType.Player)
            return;
        _actor = actor;
        _actor.ContextAreasActive++;
        _canTrigger = true;
    }

    public void OnBodyExited(Node body)
    {
        if (body != _actor)
            return;
        _actor.ContextAreasActive--;
        _actor = null;
        _canTrigger = false;
    }

    public override void _Process(double delta)
    {
        if (this.IsToolDebugMode() || !_canTrigger)
            return;
        if (!IsInstanceValid(_actor))
        {
            _actor = null;
            _canTrigger = false;
            return;
        }
        if (_actor.InputHandler.Attack.IsActionJustPressed)
            _ = Locator.Root?.GUIController.OpenDialogAsync(DialogPath);
    }
}
