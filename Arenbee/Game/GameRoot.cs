using Arenbee.GUI;
using Arenbee.GUI.Menus;
using Arenbee.Input;
using GameCore;
using GameCore.Utility;

namespace Arenbee;

public partial class GameRoot : AGameRoot
{
    public GameRoot()
    {
        GameSessionScenePath = Arenbee.GameSession.GetScenePath();
        TitleMenuScenePath = TitleMenu.GetScenePath();
        MenuInput = new MenuInputHandler();
        PlayerOneInput = new Player1InputHandler();
        TransitionController = new TransitionController();
    }

    protected override void ProvideLocatorReferences()
    {
        Locator.ProvideGameRoot(this);
        Locator.ProvideAudioController(AudioController);
        Locator.ProvideTransitionController(TransitionController);
    }
}
