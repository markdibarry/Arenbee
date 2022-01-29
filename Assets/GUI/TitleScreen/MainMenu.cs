using Arenbee.Framework.Game;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class MainMenu : SubMenu
    {
        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            switch (optionItem.Value)
            {
                case 0:
                    ContinueSavedGame();
                    break;
                case 1:
                    StartNewGame();
                    break;
            }
        }

        private void StartNewGame()
        {
            var gameRoot = GameRoot.Instance;
            gameRoot.CurrentGame = new GameSession();
            gameRoot.CurrentGameContainer.AddChild(gameRoot.CurrentGame);
            RequestedCloseAllHelper();
        }

        private void ContinueSavedGame()
        {
            var gameRoot = GameRoot.Instance;
            gameRoot.CurrentGame = new GameSession();
            GameSave gameSave = SaveService.LoadGame();
            gameRoot.CurrentGame.ApplySaveData(gameSave);
            gameRoot.CurrentGameContainer.AddChild(gameRoot.CurrentGame);
            RequestedCloseAllHelper();
        }
    }
}
