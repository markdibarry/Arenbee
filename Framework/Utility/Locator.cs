using Arenbee.Framework.Audio;
using Arenbee.Framework.Game;
using Arenbee.Framework.Input;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Framework.Utility
{
    public static class Locator
    {
        static Locator()
        {
            s_gameSession = null;
            s_itemDB = new ItemDBNull();
            s_menuInput = new MenuInputHandlerNull();
            s_statusEffectDB = new StatusEffectDBNull();
        }

        private static GameSession s_gameSession;
        private static AudioControllerNull s_audioController;
        private static IItemDB s_itemDB;
        private static GUIInputHandler s_menuInput;
        private static IStatusEffectDB s_statusEffectDB;

        public static void ProvideGameSession(GameSession gameSession)
        {
            if (Object.IsInstanceValid(s_gameSession))
                s_gameSession.Free();
            s_gameSession = gameSession;
        }

        public static void ProvideAudioController(AudioControllerNull audioController)
        {
            if (Object.IsInstanceValid(s_audioController))
                s_audioController.Free();
            s_audioController = audioController;
        }

        public static void ProvideItemDB(IItemDB itemDB)
        {
            s_itemDB = itemDB ?? new ItemDBNull();
        }

        public static void ProvideMenuInput(GUIInputHandler menuInput)
        {
            if (Object.IsInstanceValid(s_menuInput))
                s_menuInput.Free();
            s_menuInput = menuInput ?? new MenuInputHandlerNull();
        }

        public static void ProvideStatusEffectDB(IStatusEffectDB statusEffectDB)
        {
            s_statusEffectDB = statusEffectDB ?? new StatusEffectDBNull();
        }

        public static GameSession GetGameSession()
        {
            return s_gameSession;
        }

        public static AudioControllerNull GetAudio()
        {
            return s_audioController;
        }

        public static PlayerParty GetParty()
        {
            return GetGameSession()?.Party;
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