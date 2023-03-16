using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.SaveData;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class LoadGameSubMenu : OptionSubMenu
{
    private OptionContainer _loadOptions = null!;
    private GameRoot _gameRoot = (GameRoot)Locator.Root;
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnItemSelected()
    {
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(GameSave), out GameSave? loadChoice))
            return;
        ContinueSavedGame(loadChoice);
    }

    protected override void CustomSetup()
    {
        PreventCloseAll = true;
        var header = GetNode<Label>("%Header");
        header.Text = Tr(Localization.Menus.Menus_Save_SavedGames);
    }

    protected override void SetupOptions()
    {
        _loadOptions = OptionContainers.Find(x => x.Name == "LoadOptions");
        var options = GetSaveGameOptions();
        _loadOptions.ReplaceChildren(options);
    }

    private void ContinueSavedGame(GameSave gameSave)
    {
        CurrentState = State.Busy;
        async Task Callback(Loader loader)
        {
            await _gameRoot.RemoveSession();
            await _gameRoot.StartNewSession(gameSave);
        };

        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            FadeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            Array.Empty<string>(),
            Callback);
        tController.RequestTransition(request);
    }

    private static List<SaveGameOption> GetSaveGameOptions()
    {
        var saveGameOptionScene = GD.Load<PackedScene>(SaveGameOption.GetScenePath());
        List<SaveGameOption> options = new();
        foreach (GameSave gameSave in SaveService.GetGameSaves())
        {
            var option = saveGameOptionScene.Instantiate<SaveGameOption>();
            option.UpdateDisplay(gameSave);
            options.Add(option);
        }
        return options;
    }
}
