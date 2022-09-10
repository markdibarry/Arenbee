using System;
using System.Threading.Tasks;
using GameCore;
using GameCore.Extensions;
using GameCore.Game;
using GameCore.Game.SaveData;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Title;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    public MainSubMenu()
    {
        PreventCancel = true;
        PreventCloseAll = true;
    }

    private OptionContainer _startOptions;
    public static string GetScenePath() => GDEx.GetScenePath();

    public override async Task TransitionOpenAsync()
    {
        var pos = _startOptions.Position;
        _startOptions.Position = new Vector2(pos.x, -_startOptions.Size.y);
        var tween = _startOptions
            .CreateTween()
            .TweenProperty(_startOptions, "position:y", pos.y, 0.4f);
        await ToSignal(tween, "finished");
    }

    protected override void OnItemSelected()
    {
        base.OnItemSelected();
        string titleChoice = CurrentContainer.CurrentItem.GetData<string>("titleChoice");
        if (titleChoice == null)
            return;
        IsActive = false;
        switch (titleChoice)
        {
            case "Continue":
                ContinueSavedGame();
                break;
            case "NewGame":
                StartNewGame();
                break;
        }
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _startOptions = OptionContainers.Find(x => x.Name == "MainOptions");
    }

    private void StartNewGame()
    {
        Func<Loader, Task> callback = async (loader) =>
        {
            var sessionScene = loader.GetObject<PackedScene>(Locator.Root?.GameSessionScenePath);
            var gameSave = loader.GetObject<GameSave>(Config.NewGamePath);
            var session = sessionScene.Instantiate<GameSessionBase>();
            Locator.ProvideGameSession(session);
            Locator.Root?.GameSessionContainer.AddChild(session);
            session.Init(gameSave);
            await Locator.Root?.GUIController.CloseLayerAsync(new GUILayerCloseRequest()
            {
                CloseRequestType = CloseRequestType.AllLayers,
                PreventAnimation = true
            });
        };

        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            FadeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            new string[] { Locator.Root?.GameSessionScenePath, Config.NewGamePath },
            callback);
        tController.RequestTransition(request);
    }

    private void ContinueSavedGame()
    {
        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            FadeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            new string[] { Locator.Root?.GameSessionScenePath, Config.SavePath },
            async (loader) =>
            {
                var sessionScene = loader.GetObject<PackedScene>(Locator.Root?.GameSessionScenePath);
                var gameSave = loader.GetObject<GameSave>(Config.SavePath);
                var session = sessionScene.Instantiate<GameSessionBase>();
                await Locator.Root?.GUIController.CloseLayerAsync(new GUILayerCloseRequest()
                {
                    CloseRequestType = CloseRequestType.AllLayers,
                    PreventAnimation = true
                });
                Locator.ProvideGameSession(session);
                Locator.Root?.GameSessionContainer.AddChild(session);
                session.Init(gameSave);
            });
        tController.RequestTransition(request);
    }
}
