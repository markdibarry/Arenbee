using Arenbee.GUI;
using Arenbee.GUI.Menus;
using Arenbee.Input;
using GameCore;

namespace Arenbee;

public partial class GameRoot : BaseGameRoot
{
    public GameRoot()
        : base(Arenbee.GameSession.GetScenePath(), TitleMenu.GetScenePath())
    {
    }

    public override MenuInputHandler MenuInput { get; } = new();
    public override Player1InputHandler PlayerOneInput { get; } = new();

    protected override void StartRoot()
    {
        ResetToTitleScreen(string.Empty, FadeTransition.GetScenePath(), string.Empty);
    }
}
