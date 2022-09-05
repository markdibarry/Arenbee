using Arenbee.ActionEffects;
using Arenbee.GUI;
using Arenbee.GUI.Menus;
using Arenbee.Input;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Game;
using GameCore.Utility;

namespace Arenbee.Game;

public partial class GameRoot : GameRootBase
{
    public GameRoot()
    {
        GameSessionScenePath = GameSession.GetScenePath();
        TitleMenuScenePath = TitleMenu.GetScenePath();
        MenuInput = new MenuInputHandler();
        PlayerOneInput = new Player1InputHandler();
        TransitionController = new TransitionController();
    }

    protected override void ProvideLocatorReferences()
    {
        Locator.ProvideGameRoot(this);
        Locator.ProvideAudioController(AudioController);
        Locator.ProvideItemDB(new ItemDB());
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideStatusEffectDB(new StatusEffectDB());
        Locator.ProvideTransitionController(TransitionController);
    }
}
