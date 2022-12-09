using System.Collections.Generic;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.SaveData;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveGameSubMenu : OptionSubMenu
{
    private OptionContainer _saveOptions;
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnItemSelected()
    {
        var saveChoice = CurrentContainer.CurrentItem.GetData<GameSave>(nameof(GameSave));
        OpenSaveGameConfirmSubMenu(saveChoice);
    }

    protected override void SetupOptions()
    {
        _saveOptions = OptionContainers.Find(x => x.Name == "SaveOptions");
        var options = GetSaveGameOptions();
        _saveOptions.ReplaceChildren(options);
    }

    private void OpenSaveGameConfirmSubMenu(GameSave gameSave)
    {
        _ = OpenSubMenuAsync(path: SaveConfirmSubMenu.GetScenePath(), data: gameSave.Id);
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
