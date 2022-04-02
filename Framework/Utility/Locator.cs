using Arenbee.Framework.Game;
using Arenbee.Framework.Input;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Utility
{
    public static class Locator
    {
        static Locator()
        {
            s_currentGame = new GameSessionNull();
            s_itemDB = new ItemDBNull();
            s_menuInput = new MenuInputHandlerNull();
            s_statusEffectDB = new StatusEffectDBNull();
        }

        private static GameSessionBase s_currentGame;
        private static IItemDB s_itemDB;
        private static GUIInputHandler s_menuInput;
        private static IStatusEffectDB s_statusEffectDB;

        public static void ProvideCurrentGame(GameSessionBase gameSession)
        {
            if (Godot.Object.IsInstanceValid(s_currentGame))
                s_currentGame.Free();
            s_currentGame = gameSession ?? new GameSessionNull();
        }

        public static void ProvideItemDB(IItemDB itemDB)
        {
            s_itemDB = itemDB ?? new ItemDBNull();
        }

        public static void ProvideMenuInput(GUIInputHandler menuInput)
        {
            if (Godot.Object.IsInstanceValid(s_menuInput))
                s_menuInput.Free();
            s_menuInput = menuInput ?? new MenuInputHandlerNull();
        }

        public static void ProvideStatusEffectDB(IStatusEffectDB statusEffectDB)
        {
            s_statusEffectDB = statusEffectDB ?? new StatusEffectDBNull();
        }

        public static GameSessionBase GetCurrentGame()
        {
            return s_currentGame;
        }

        public static IItemDB GetItemDB()
        {
            return s_itemDB;
        }

        public static GUIInputHandler GetMenuInput()
        {
            return s_menuInput;
        }

        public static IStatusEffectDB GetStatusEffectDB()
        {
            return s_statusEffectDB;
        }
    }
}