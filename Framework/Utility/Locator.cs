using Arenbee.Framework.ActionEffects;
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
            s_logger = new Logger();
            s_actionEffectDB = new ActionEffectDBNull();
        }

        private static GameSession s_gameSession;
        private static AudioControllerNull s_audioController;
        private static IItemDB s_itemDB;
        private static readonly Logger s_logger;
        private static GUIInputHandler s_menuInput;
        private static IStatusEffectDB s_statusEffectDB;
        private static IActionEffectDB s_actionEffectDB;

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

        public static void ProvideActionEffectDB(IActionEffectDB actionEffectDB)
        {
            s_actionEffectDB = actionEffectDB ?? new ActionEffectDBNull();
        }

        public static IActionEffectDB GetActionEffectDB() => s_actionEffectDB;

        public static GameSession GetGameSession() => s_gameSession;

        public static AudioControllerNull GetAudio() => s_audioController;

        public static PlayerParty GetParty() => GetGameSession()?.Party;

        public static IItemDB GetItemDB() => s_itemDB;

        public static GUIInputHandler GetMenuInput() => s_menuInput;

        public static IStatusEffectDB GetStatusEffectDB() => s_statusEffectDB;

        public static Logger GetLogger() => s_logger;
    }
}