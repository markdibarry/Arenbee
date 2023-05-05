using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.SaveData;
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

    protected override void OnSelectPressed()
    {
        if (CurrentContainer?.FocusedItem?.OptionData is not string fileName)
            return;
        ContinueSavedGame(fileName);
    }

    protected override void CustomSetup()
    {
        PreventCloseAll = true;
        var header = GetNode<Label>("%Header");
        header.Text = this.TrS(Localization.Menus.Menus_Save_SavedGames);
        List<SaveGameOption> options = GetSaveGameOptions();
        _loadOptions.ReplaceChildren(options);
    }

    protected override void SetNodeReferences()
    {
        _loadOptions = GetNode<OptionContainer>("%LoadOptions");
        AddContainer(_loadOptions);
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

        ATransitionController tController = Locator.TransitionController;
        TransitionRequest request = new(
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
