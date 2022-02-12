using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class SaveConfirmSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        protected override async void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            switch (optionItem.OptionValue)
            {
                case "Yes":
                    SaveGame();
                    break;
                case "No":
                    await CloseSubMenu();
                    break;
            }
        }

        private void SaveGame()
        {
            SaveService.SaveGame(GameRoot.Instance.CurrentGame);
            RaiseRequestedAddSubMenu(GDEx.Instantiate<SaveSuccessSubMenu>(SaveSuccessSubMenu.GetScenePath()));
        }
    }
}
