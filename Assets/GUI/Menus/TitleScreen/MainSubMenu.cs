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

        public static new readonly string ScenePath = $"res://Assets/GUI/Menus/TitleScreen/{nameof(MainSubMenu)}.tscn";

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            switch (optionItem.OptionValue)
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
            gameRoot.CurrentGame = new GameSession(SaveService.LoadGame());
            gameRoot.CurrentGameContainer.AddChild(gameRoot.CurrentGame);
            RaiseRequestedCloseAll();
        }
    }
}
