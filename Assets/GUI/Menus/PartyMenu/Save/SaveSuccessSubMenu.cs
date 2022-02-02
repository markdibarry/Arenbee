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

        public static new readonly string ScenePath = $"res://Assets/GUI/Menus/PartyMenu/Save/{nameof(SaveSuccessSubMenu)}.tscn";

        protected override void OnTimeOut()
        {
            base.OnTimeOut();
            CloseSubMenu(nameof(MainSubMenu));
        }
    }
}
