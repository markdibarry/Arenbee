using Arenbee.GUI.Menus;
using GameCore.AreaScenes;
using GameCore.Constants;
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

    public override void HandleInput(GUIInputHandler menuInput, float delta)
    {
        if (menuInput.Start.IsActionJustPressed && !GUIController.GUIActive)
            OpenPartyMenuAsync();
    }

    private void InitAreaScene()
    {
        // TODO: Make game
        if (CurrentAreaScene == null)
        {
            var demoAreaScene = GDEx.Instantiate<AreaScene>(Arenbee.Constants.PathConstants.DemoLevel1);
            AddAreaScene(demoAreaScene);
        }
    }

    private async void OpenPartyMenuAsync()
    {
        await GUIController.OpenMenuAsync(_partyMenuScene);
    }
}
