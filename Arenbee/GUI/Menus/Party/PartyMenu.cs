using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus;

[Tool]
public partial class PartyMenu : Menu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public override async Task AnimateOpenAsync()
    {
        ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
        Tween tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 1f, 0.2f);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public override async Task AnimateCloseAsync()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 0f, 0.2f);
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}
