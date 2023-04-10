using Arenbee.GUI;
using Arenbee.GUI.Menus;
using Arenbee.Input;
using GameCore;

namespace Arenbee;

public partial class GameRoot : AGameRoot
{
    public GameRoot()
        : base(Arenbee.GameSession.GetScenePath(), TitleMenu.GetScenePath())
    {
        MenuInput = new();
        PlayerOneInput = new();
        TransitionController = new();
    }

    public override MenuInputHandler MenuInput { get; }
    public override Player1InputHandler PlayerOneInput { get; }
    public override TransitionController TransitionController { get; }

    protected override void StartRoot()
    {
        ResetToTitleScreen(string.Empty, FadeTransition.GetScenePath(), string.Empty);
    }
}
