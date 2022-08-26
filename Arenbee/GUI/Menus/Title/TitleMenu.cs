using Arenbee.GUI.Menus.Title;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;
using System.Threading.Tasks;

namespace Arenbee.GUI.Menus
{
    [Tool]
    public partial class TitleMenu : Menu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly PackedScene _mainSubMenuScene = GD.Load<PackedScene>(MainSubMenu.GetScenePath());

        public override async Task TransitionOpenAsync()
        {
            ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
            using var tween = GetTree().CreateTween();
            using var prop = tween.TweenProperty(ContentGroup, "self_modulate:a", 1f, 1f);
            await ToSignal(tween, "finished");
            var mainSubMenu = _mainSubMenuScene.Instantiate<SubMenu>();
            await AddSubMenuAsync(mainSubMenu);
        }

        public override async Task TransitionCloseAsync()
        {
            using var tween = GetTree().CreateTween();
            using var prop = tween.TweenProperty(ContentGroup, "self_modulate:a", 0f, 1f);
            await ToSignal(tween, "finished");
        }
    }
}
