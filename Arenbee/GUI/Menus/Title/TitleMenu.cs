using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus;

[Tool]
public partial class TitleMenu : Menu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public override async Task AnimateOpenAsync()
    {
        ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
        var tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 1, 1);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public override async Task AnimateCloseAsync()
    {
        var tween = CreateTween();
        tween.TweenProperty(ContentGroup, "self_modulate:a", 0, 1);
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}
