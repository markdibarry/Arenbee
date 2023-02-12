using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace GameCore.Events;

public partial class DialogArea : Area2D, IContextArea
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private ColorRect _colorRect = null!;
    private string _dialogPath = string.Empty;
    private GUIController? _guiController = Locator.Root?.GUIController;
    [Export(PropertyHint.File, "*.json")]
    public string DialogPath
    {
        get => _dialogPath;
        set
        {
            _dialogPath = value;
            if (!FileAccess.FileExists(value))
                IsActive = false;
        }
    }
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
        if (body is not AActorBody actor || actor.ActorType != ActorType.Player)
            return;
        actor.ContextAreas.Add(this);
    }

    public void OnBodyExited(Node body)
    {
        if (body is not AActorBody actor || actor.ActorType != ActorType.Player)
            return;
        actor.ContextAreas.Remove(this);
    }

    public void TriggerContext(AActorBody actor)
    {
        if (this.IsToolDebugMode() || !IsActive || !actor.InputHandler.Attack.IsActionJustPressed)
            return;
        _ = _guiController?.OpenDialogAsync(DialogPath);
    }
}
