using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI;

public partial class WipeTransition : Transition
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private ColorRect _colorRect;
    private ColorRect _colorRect2;

    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("ColorRect");
        _colorRect2 = GetNode<ColorRect>("ColorRect2");
    }

    public override async Task TransistionFrom()
    {
        _colorRect.Position = new Vector2(480, 0);
        _colorRect2.Position = new Vector2(0, 270);
        var tween2 = GetTree().CreateTween();
        var prop2 = tween2.TweenProperty(_colorRect2, "position:y", 0, 0.8f);
        await _colorRect2.ToSignal(tween2, "finished");

        var tween = GetTree().CreateTween();
        var prop = tween.TweenProperty(_colorRect, "position:x", 0, 0.8f);
        await _colorRect.ToSignal(tween, "finished");
    }

    public override async Task TransitionTo()
    {
        var tween = GetTree().CreateTween();
        var prop = tween.TweenProperty(_colorRect, "position:x", -480, 0.8f);
        await _colorRect.ToSignal(tween, "finished");
        var tween2 = GetTree().CreateTween();
        var prop2 = tween2.TweenProperty(_colorRect2, "position:y", -270, 0.8f);
        await _colorRect2.ToSignal(tween2, "finished");
    }
}
