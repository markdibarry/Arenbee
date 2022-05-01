using Arenbee.Assets.GUI.Menus.Title;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;
using System.Threading.Tasks;

namespace Arenbee.Assets.GUI.Menus
{
    [Tool]
    public partial class TitleMenu : Menu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly PackedScene _mainSubMenuScene = GD.Load<PackedScene>(MainSubMenu.GetScenePath());

        public override async Task TransitionOpenAsync()
        {
            Visible = true;
            ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
            var tween = GetTree().CreateTween();
            tween.TweenProperty(ContentGroup, "self_modulate:a", 1f, 1f);
            await ToSignal(tween, "finished");

            var mainSubMenu = _mainSubMenuScene.Instantiate<SubMenu>();
            await AddSubMenuAsync(mainSubMenu);
        }

        public override async Task TransitionCloseAsync()
        {
            var tween = GetTree().CreateTween();
            tween.TweenProperty(ContentGroup, "self_modulate:a", 0f, 1f);
            await ToSignal(tween, "finished");
            Visible = false;
        }
    }
}
