﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GameCore;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.SaveData;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class LoadGameSubMenu : OptionSubMenu
{
    private OptionContainer _loadOptions;
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnItemSelected()
    {
        var loadChoice = CurrentContainer.CurrentItem.GetData<GameSave>(nameof(GameSave));
        ContinueSavedGame(loadChoice);
    }

    protected override void SetupOptions()
    {
        _loadOptions = OptionContainers.Find(x => x.Name == "LoadOptions");
        var options = GetSaveGameOptions();
        _loadOptions.ReplaceChildren(options);
    }

    private void ContinueSavedGame(GameSave gameSave)
    {
        Loading = true;
        async Task Callback(Loader loader)
        {
            var sessionScene = loader.GetObject<PackedScene>(Locator.Root?.GameSessionScenePath);
            var session = sessionScene.Instantiate<GameSessionBase>();
            await Locator.Root?.GUIController.CloseLayerAsync(new GUICloseRequest()
            {
                CloseRequestType = CloseRequestType.AllLayers,
                PreventAnimation = true
            });
            Locator.ProvideGameSession(session);
            Locator.Root?.GameSessionContainer.AddChild(session);
            session.Init(gameSave);
        };

        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            FadeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            new string[] { Locator.Root?.GameSessionScenePath},
            Callback);
        tController.RequestTransition(request);
    }

    private List<SaveGameOption> GetSaveGameOptions()
    {
        var saveGameOptionScene = GD.Load<PackedScene>(SaveGameOption.GetScenePath());
        var gameSaves = SaveService.GetGameSaves();
        List<SaveGameOption> options = new();
        foreach (var gameSave in gameSaves)
        {
            var option = saveGameOptionScene.Instantiate<SaveGameOption>();
            option.UpdateDisplay(gameSave);
            options.Add(option);
        }
        return options;
    }
}