using GameCore.Constants;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class TimedMessageBox : MessageBox
{
    public static new string GetScenePath() => GDEx.GetScenePath();
    [Export]
    private double _timeOut = 2.0;
    private bool _timerFinished;

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            return;
        base._PhysicsProcess(delta);
        if (_timerFinished)
            return;
        if (_timeOut > 0)
        {
            _timeOut -= delta;
        }
        else
        {
            _timerFinished = true;
            TransitionOut();
        }
    }

    public async void TransitionOut()
    {
        Tween fadeTween = GetTree().CreateTween();
        fadeTween.TweenProperty(this, "modulate:a", 0f, 0.1f);
        await ToSignal(fadeTween, GodotConstants.FinishedSignal);
        QueueFree();
    }
}
