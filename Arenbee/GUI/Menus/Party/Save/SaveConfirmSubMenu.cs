using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Game.SaveData;
using Godot;
using GameCore.Utility;

namespace Arenbee.GUI.Menus.Party
{
    [Tool]
    public partial class SaveConfirmSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        protected override void OnItemSelected()
        {
            base.OnItemSelected();
            var saveChoice = CurrentContainer.CurrentItem.GetData<string>("saveChoice");
            if (saveChoice == null)
                return;
            switch (saveChoice)
            {
                case "Yes":
                    SaveGame();
                    break;
                case "No":
                    CloseSubMenu();
                    break;
            }
        }

        private void SaveGame()
        {
            SaveService.SaveGame(Locator.Session);
            RaiseRequestedAdd(GDEx.Instantiate<SaveSuccessSubMenu>(SaveSuccessSubMenu.GetScenePath()));
        }
    }
}
