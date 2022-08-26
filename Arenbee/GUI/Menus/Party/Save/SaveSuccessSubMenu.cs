using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus.Party
{
    [Tool]
    public partial class SaveSuccessSubMenu : PromptSubMenu
    {
        public SaveSuccessSubMenu()
        {
            PreventCloseAll = true;
            PreventCancel = true;
        }

        public static string GetScenePath() => GDEx.GetScenePath();

        protected override void OnTimeOut()
        {
            base.OnTimeOut();
            RaiseRequestedClose(new SubMenuCloseRequest(nameof(MainSubMenu)));
        }
    }
}
