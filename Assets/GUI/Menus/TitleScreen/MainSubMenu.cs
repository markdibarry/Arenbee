using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Assets.GUI.Menus.TitleMenus
{
    [Tool]
    public partial class MainSubMenu : OptionSubMenu
    {
        public MainSubMenu()
            : base()
        {
            PreventCancel = true;
            PreventCloseAll = true;
        }

        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            switch (optionItem.Value)
            {
                case "Continue":
                    ContinueSavedGame();
                    break;
                case "NewGame":
                    StartNewGame();
                    break;
            }
        }

        private void StartNewGame()
        {
            var gameRoot = GameRoot.Instance;
            gameRoot.CurrentGame = new GameSession();
            gameRoot.CurrentGameContainer.AddChild(gameRoot.CurrentGame);
            RaiseRequestedCloseAll();
        }

        private void ContinueSavedGame()
        {
            var gameRoot = GameRoot.Instance;
            gameRoot.CurrentGame = new GameSession();
            GameSave gameSave = SaveService.LoadGame();
            gameRoot.CurrentGame.ApplySaveData(gameSave);
            gameRoot.CurrentGameContainer.AddChild(gameRoot.CurrentGame);
            RaiseRequestedCloseAll();
        }
    }
}
