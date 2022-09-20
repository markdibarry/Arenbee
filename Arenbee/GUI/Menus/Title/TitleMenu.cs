using GameCore;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;
using System.Threading.Tasks;

namespace Arenbee.GUI.Menus;

[Tool]
public partial class TitleMenu : Menu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public override async Task TransitionOpenAsync()
    {
        ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
        var tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 1f, 1f);
        await ToSignal(tween, Signals.FinishedSignal);
    }

    public override async Task TransitionCloseAsync()
    {
        var tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 0f, 1f);
        await ToSignal(tween, Signals.FinishedSignal);
    }
}
