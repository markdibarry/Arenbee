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

        public override void OnTimeOut()
        {
            base.OnTimeOut();
            CloseSubMenu(nameof(MainSubMenu));
        }
    }
}
