using GameCore.ActionEffects;
using GameCore.Audio;
using GameCore.Game;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace GameCore.Utility
{
    public static class Locator
    {
        static Locator()
        {
            s_actionEffectDB = new ActionEffectDBNull();
            s_audioController = new AudioControllerNull();
            s_itemDB = new ItemDBNull();
            s_logger = new Logger();
            s_gameRoot = null;
            s_gameSession = null;
            s_statusEffectDB = new StatusEffectDBNull();
            s_transitionController = new TransitionControllerNull();
        }

        private static GameRootBase s_gameRoot;
        private static GameSessionBase s_gameSession;
        private static AudioControllerBase s_audioController;
        private static ItemDBBase s_itemDB;
        private static readonly Logger s_logger;
        private static StatusEffectDBBase s_statusEffectDB;
        private static ActionEffectDBBase s_actionEffectDB;
        private static TransitionControllerBase s_transitionController;


        public static void ProvideActionEffectDB(ActionEffectDBBase actionEffectDB)
        {
            s_actionEffectDB = actionEffectDB ?? new ActionEffectDBNull();
        }

        public static void ProvideAudioController(AudioControllerBase audioController)
        {
            if (Object.IsInstanceValid(s_audioController))
                s_audioController.Free();
            s_audioController = audioController;
        }

        public static void ProvideItemDB(ItemDBBase itemDB)
        {
            s_itemDB = itemDB ?? new ItemDBNull();
        }

        public static void ProvideGameRoot(GameRootBase gameRoot)
        {
            if (Object.IsInstanceValid(s_gameRoot))
                s_gameRoot.Free();
            s_gameRoot = gameRoot;
        }

        public static void ProvideGameSession(GameSessionBase gameSession)
        {
            if (Object.IsInstanceValid(s_gameSession))
                s_gameSession.Free();
            s_gameSession = gameSession;
        }

        public static void ProvideStatusEffectDB(StatusEffectDBBase statusEffectDB)
        {
            s_statusEffectDB = statusEffectDB ?? new StatusEffectDBNull();
        }

        public static void ProvideTransitionController(TransitionControllerBase transitionController)
        {
            s_transitionController = transitionController ?? new TransitionControllerNull();
        }

        public static ActionEffectDBBase ActionEffectDB => s_actionEffectDB;

        public static AudioControllerBase Audio => s_audioController;

        public static ItemDBBase ItemDB => s_itemDB;

        public static Logger GetLogger() => s_logger;

        public static PlayerParty GetParty() => Session?.Party;

        public static GameRootBase Root => s_gameRoot;

        public static GameSessionBase Session => s_gameSession;

        public static StatusEffectDBBase StatusEffectDB => s_statusEffectDB;

        public static TransitionControllerBase TransitionController => s_transitionController;
    }
}
