using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI;

public partial class FadeTransition : Transition
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private ColorRect _colorRect;

    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("ColorRect");
    }

    public override async Task TransistionFrom()
    {
        _colorRect.Modulate = Colors.Transparent;
        var tween = GetTree().CreateTween()
            .TweenProperty(_colorRect, "modulate:a", 1f, 0.4f);
        await _colorRect.ToSignal(tween, "finished");
    }

    public override async Task TransitionTo()
    {
        var tween = GetTree().CreateTween()
            .TweenProperty(_colorRect, "modulate:a", 0f, 0.4f);
        await _colorRect.ToSignal(tween, "finished");
    }
}
