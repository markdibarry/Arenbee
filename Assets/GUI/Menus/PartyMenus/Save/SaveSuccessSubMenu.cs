using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class SaveSuccessSubMenu : PromptSubMenu
    {
        public SaveSuccessSubMenu()
            : base()
        {
            PreventCloseAll = true;
            PreventCancel = true;
        }

        public static string GetScenePath() => GDEx.GetScenePath();

        protected override async void OnTimeOut()
        {
            base.OnTimeOut();
            await CloseSubMenu(nameof(MainSubMenu));
        }
    }
}
