using Arenbee.Framework.GUI;
using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Game
{
    public class GameState
    {
        public bool CutsceneActive { get; private set; }
        public bool LoadingActive { get; private set; }
        public bool MenuActive { get; private set; }
        public bool DialogActive { get; private set; }
        public event GameStateChangedHandler GameStateChanged;
        public delegate void GameStateChangedHandler(GameState gameState);

        public void Init(GUIController guiController)
        {
            guiController.GUIStatusChanged += OnGUIStatusChanged;
        }

        public void OnGUIStatusChanged(GUIController guiController)
        {
            var session = Locator.GetGameSession();
            if (session == null)
                return;
            MenuActive = guiController.MenuActive;
            CutsceneActive = guiController.DialogActive;
            GameStateChanged?.Invoke(this);
        }
    }
}