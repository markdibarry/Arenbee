using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
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
        private GameSessionBase _currentGame;
        private OptionContainer _saveOptions;

        protected override void CustomOptionsSetup()
        {
            base.CustomOptionsSetup();
            _currentGame = Locator.GetCurrentGame();
        }

        protected override async void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (!optionItem.OptionData.TryGetValue("saveChoice", out string saveChoice))
                return;
            switch (saveChoice)
            {
                case "Yes":
                    SaveGame();
                    break;
                case "No":
                    await CloseSubMenuAsync();
                    break;
            }
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _saveOptions = Foreground.GetNode<OptionContainer>("SaveOptions");
            OptionContainers.Add(_saveOptions);
        }

        private void SaveGame()
        {
            SaveService.SaveGame(_currentGame);
            RaiseRequestedAddSubMenu(GDEx.Instantiate<SaveSuccessSubMenu>(SaveSuccessSubMenu.GetScenePath()));
        }
    }
}
