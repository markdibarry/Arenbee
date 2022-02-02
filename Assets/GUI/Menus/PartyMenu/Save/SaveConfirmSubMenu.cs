using Arenbee.Framework.Constants;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class SaveConfirmSubMenu : OptionSubMenu
    {
        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            switch (optionItem.Value)
            {
                case "Yes":
                    SaveGame();
                    break;
                case "No":
                    CloseSubMenu();
                    break;
                default:
                    break;
            }
        }

        public void SaveGame()
        {
            SaveService.SaveGame(GameRoot.Instance.CurrentGame);
            var saveSuccessScene = GD.Load<PackedScene>(PathConstants.SaveSuccessSubMenuPath);
            RaiseRequestedAddSubMenu(saveSuccessScene.Instantiate<SubMenu>());
        }
    }
}
