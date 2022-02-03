using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class SaveConfirmSubMenu : OptionSubMenu
    {
        public static new readonly string ScenePath = $"res://Assets/GUI/Menus/PartyMenu/Save/{nameof(SaveConfirmSubMenu)}.tscn";
        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            switch (optionItem.OptionValue)
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

        private void SaveGame()
        {
            SaveService.SaveGame(GameRoot.Instance.CurrentGame);
            var saveSuccessScene = GD.Load<PackedScene>(SaveSuccessSubMenu.ScenePath);
            RaiseRequestedAddSubMenu(saveSuccessScene.Instantiate<SubMenu>());
        }
    }
}
