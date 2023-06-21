using Arenbee.GUI;
using Arenbee.GUI.Menus;
using Arenbee.Input;
using GameCore;
using GameCore.GUI;
using GameCore.Input;

namespace Arenbee;

public partial class GameRoot : BaseGameRoot
{
    public GameRoot()
        : base(Arenbee.GameSession.GetScenePath(), TitleMenu.GetScenePath())
    {
        DialogBridgeRegister.SetDialogBridge(new DialogBridge());
    }

    public override IGUIInputHandler MenuInput { get; } = InputFactory.CreateMenuInput();
    public override IActorInputHandler PlayerOneInput { get; } = InputFactory.CreatePlayer1Input();

    protected override void StartRoot()
    {
        ResetToTitleScreen(string.Empty, FadeTransition.GetScenePath(), string.Empty);
    }
}
