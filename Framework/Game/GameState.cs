using Arenbee.Framework.GUI;
using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Game
{
    public class GameState
    {
        public bool CutsceneActive { get; private set; }
        public bool LoadingActive { get; private set; }
        public bool MenuActive { get; private set; }

        public void Init(MenuController menuController)
        {
            menuController.MenuStatusChanged += OnMenuStatusChanged;
        }

        public void OnMenuStatusChanged(bool isActive)
        {
            MenuActive = isActive;
            var session = Locator.GetGameSession();
            if (session == null)
                return;
            if (isActive)
                session.Pause();
            else
                session.Resume();
        }

        public void OnLoadingStatusChanged(bool isActive)
        {
            LoadingActive = isActive;
        }

        public void OnCutsceneStatusChanged(bool isActive)
        {
            CutsceneActive = isActive;
        }
    }
}