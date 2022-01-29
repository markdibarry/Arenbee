using Arenbee.Framework.Constants;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class MainSubMenu : SubMenu
    {
        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            switch (optionItem.Value)
            {
                case "Save":
                    OpenSaveGameConfirm();
                    break;
                case "Quit":
                    GameRoot.Instance.ResetToTitleScreen();
                    break;
                default:
                    break;
            }
        }

        public void OpenSaveGameConfirm()
        {
            var saveConfirmScene = GD.Load<PackedScene>(PathConstants.SaveConfirmSubMenuPath);
            RaiseRequestedAddSubMenu(saveConfirmScene.Instantiate<SubMenu>());
        }
    }
}
