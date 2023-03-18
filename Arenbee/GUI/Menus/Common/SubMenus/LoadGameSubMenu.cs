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
        if (CurrentContainer?.FocusedItem == null || !CurrentContainer.FocusedItem.TryGetData("fileName", out string? fileName))
            return;
        ContinueSavedGame(fileName);
    }

    protected override void CustomSetup()
    {
        PreventCloseAll = true;
        var header = GetNode<Label>("%Header");
        header.Text = Tr(Localization.Menus.Menus_Save_SavedGames);
    }

    protected override void SetupOptions()
    {
        _loadOptions = OptionContainers.Find(x => x.Name == "LoadOptions")!;
        var options = GetSaveGameOptions();
        _loadOptions.ReplaceChildren(options);
    }

    private void ContinueSavedGame(string fileName)
    {
        CurrentState = State.Busy;
        async Task Callback(Loader loader)
        {
            await _gameRoot.RemoveSession();
            GameSave? gameSave = SaveService.GetGameSave(fileName);
            await _gameRoot.StartNewSession(gameSave!);
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
        PackedScene saveGameOptionScene = GD.Load<PackedScene>(SaveGameOption.GetScenePath());
        List<(string, GameSave)> gameSaves = SaveService.GetAllSaves();
        List<SaveGameOption> options = new();
        for (int i = 0; i < gameSaves.Count; i++)
        {
            SaveGameOption option = saveGameOptionScene.Instantiate<SaveGameOption>();
            option.GameNameText = $"File {i + 1}";
            option.UpdateDisplay(gameSaves[i].Item2, gameSaves[i].Item1);
            options.Add(option);
        }
        return options;
    }
}
