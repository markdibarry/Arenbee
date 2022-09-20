using Arenbee.GUI.Menus;
using GameCore;
using GameCore.AreaScenes;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Input;

namespace Arenbee;

public partial class GameSession : GameSessionBase
{
    public static string GetScenePath() => GDEx.GetScenePath();

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
            var demoAreaScene = GDEx.Instantiate<AreaScene>(Paths.DemoLevel1);
            AddAreaScene(demoAreaScene);
        }
    }

    private async void OpenPartyMenuAsync()
    {
        GUIOpenRequest request = new(PartyMenu.GetScenePath());
        await GUIController.OpenLayerAsync(request);
    }
}
