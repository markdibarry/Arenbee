using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Game.SaveData;
using Godot;
using Arenbee.Framework.Utility;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class SaveConfirmSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            var saveChoice = optionItem.GetData<string>("saveChoice");
            if (saveChoice == null)
                return;
            switch (saveChoice)
            {
                case "Yes":
                    SaveGame();
                    break;
                case "No":
                    RaiseRequestedClose();
                    break;
            }
        }

        private void SaveGame()
        {
            SaveService.SaveGame(Locator.GetGameSession());
            RaiseRequestedAdd(GDEx.Instantiate<SaveSuccessSubMenu>(SaveSuccessSubMenu.GetScenePath()));
        }
    }
}
