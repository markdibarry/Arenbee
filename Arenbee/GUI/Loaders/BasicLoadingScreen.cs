using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI;

[Tool]
public partial class BasicLoadingScreen : LoadingScreen
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public override void _Ready()
    {
        base._Ready();
        var sprite2d = GetNode<Sprite2D>("Sprite2d");
        var tween = sprite2d.CreateTween().SetLoops();
        var prop2 = tween.TweenProperty(sprite2d, "rotation", Mathf.Tau, 1f)
            .AsRelative();
    }
}
