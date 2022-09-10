using Arenbee.GUI.Menus;
using GameCore.AreaScenes;
using GameCore.Extensions;
using GameCore.Game;
using GameCore.GUI;
using GameCore.Input;
using Godot;

namespace Arenbee.Game;

public partial class GameSession : GameSessionBase
{
    public GameSession()
        : base()
    {
        _partyMenuScene = GD.Load<PackedScene>(PartyMenu.GetScenePath());
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly PackedScene _partyMenuScene;

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Start.IsActionJustPressed && !GUIController.GUIActive)
            OpenPartyMenuAsync();
    }

    private void InitAreaScene()
    {
        // TODO: Make game
        if (CurrentAreaScene == null)
        {
            var demoAreaScene = GDEx.Instantiate<AreaScene>(Constants.PathConstants.DemoLevel1);
            AddAreaScene(demoAreaScene);
        }
    }

    private async void OpenPartyMenuAsync()
    {
        MenuOpenRequest request = new(_partyMenuScene);
        await GUIController.OpenMenuAsync(request);
    }
}
