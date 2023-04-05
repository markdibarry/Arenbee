using Arenbee.GUI;
using Arenbee.GUI.Menus;
using Arenbee.Input;
using GameCore;
using GameCore.GUI;
using GameCore.Input;

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

    public override GUIInputHandler MenuInput { get; }
    public override InputHandler PlayerOneInput { get; }
    public override TransitionControllerBase TransitionController { get; }

    protected override void StartRoot()
    {
        ResetToTitleScreen(string.Empty, FadeTransition.GetScenePath(), string.Empty);
    }
}
