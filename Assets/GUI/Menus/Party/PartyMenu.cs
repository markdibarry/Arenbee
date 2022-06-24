using Arenbee.Assets.GUI.Menus.Party;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;
using System.Threading.Tasks;

namespace Arenbee.Assets.GUI.Menus
{
    [Tool]
    public partial class PartyMenu : Menu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly PackedScene _mainSubMenuScene = GD.Load<PackedScene>(MainSubMenu.GetScenePath());

        public override async void _Ready()
        {
            base._Ready();
            var mainSubMenu = _mainSubMenuScene.Instantiate<SubMenu>();
            await AddSubMenuAsync(mainSubMenu);
        }

        public override async Task TransitionOpenAsync()
        {
            ContentGroup.SelfModulate = new Color(ContentGroup.SelfModulate, 0);
            using Tween tween = GetTree().CreateTween();
            using PropertyTweener prop = tween.TweenProperty(ContentGroup, "self_modulate:a", 1f, 0.1f);
            await ToSignal(tween, "finished");
        }

        public override async Task TransitionCloseAsync()
        {
            using Tween tween = GetTree().CreateTween();
            using PropertyTweener prop = tween.TweenProperty(ContentGroup, "self_modulate:a", 0f, 0.1f);
            await ToSignal(tween, "finished");
        }
    }
}
