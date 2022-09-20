using System.Threading.Tasks;
using GameCore;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus;

[Tool]
public partial class PartyMenu : Menu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public override async Task TransitionOpenAsync()
    {
        ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
        Tween tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 1f, 0.2f);
        await ToSignal(tween, Signals.FinishedSignal);
    }

    public override async Task TransitionCloseAsync()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 0f, 0.2f);
        await ToSignal(tween, Signals.FinishedSignal);
    }
}
