using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI;

public partial class WipeTransition : Transition
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private ColorRect _colorRect = null!;
    private ColorRect _colorRect2 = null!;

    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("ColorRect");
        _colorRect2 = GetNode<ColorRect>("ColorRect2");
    }

    public override async Task TransistionFrom()
    {
        _colorRect.Position = new Vector2(480, 0);
        _colorRect2.Position = new Vector2(0, 270);
        var tween2 = _colorRect2.CreateTween();
        tween2.TweenProperty(_colorRect2, "position:y", 0, 0.8f)
            .SetEase(Tween.EaseType.Out);
        await _colorRect2.ToSignal(tween2, Tween.SignalName.Finished);

        var tween = _colorRect.CreateTween();
        tween.TweenProperty(_colorRect, "position:x", 0, 0.8f);
        await _colorRect.ToSignal(tween, Tween.SignalName.Finished);
    }

    public override async Task TransitionTo()
    {
        var tween = _colorRect.CreateTween();
        tween.TweenProperty(_colorRect, "position:x", -480, 0.8f);
        await _colorRect.ToSignal(tween, Tween.SignalName.Finished);

        var tween2 = _colorRect2.CreateTween();
        tween2.TweenProperty(_colorRect2, "position:y", -270, 0.8f)
            .SetEase(Tween.EaseType.Out);
        await _colorRect2.ToSignal(tween2, Tween.SignalName.Finished);
    }
}
