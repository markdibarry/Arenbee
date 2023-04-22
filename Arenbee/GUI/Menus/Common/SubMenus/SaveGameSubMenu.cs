using System.Collections.Generic;
using Arenbee.SaveData;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveGameSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private List<(string, GameSave)> _gameSaves = new();
    private PackedScene _saveGameOptionScene = GD.Load<PackedScene>(SaveGameOption.GetScenePath());

    protected override void OnItemSelected()
    {
        string? fileName = CurrentContainer?.FocusedItem?.OptionData as string;
        OpenSaveGameConfirmSubMenu(fileName);
    }

    public override void UpdateData(object? data)
    {
        SetupOptions();
    }

    protected override void CustomSetup()
    {
        var header = GetNode<Label>("%Header");
        header.Text = Tr(Localization.Menus.Menus_Save_SavedGames);
    }

    protected override void SetupOptions()
    {
        OptionContainer? saveOptions = OptionContainers.Find(x => x.Name == "SaveOptions");
        List<SaveGameOption> options = GetSaveGameOptions();
        saveOptions?.ReplaceChildren(options);
    }

    private void OpenSaveGameConfirmSubMenu(string? fileName)
    {
        _ = OpenSubMenuAsync(path: SaveConfirmSubMenu.GetScenePath(), data: fileName);
    }

    private List<SaveGameOption> GetSaveGameOptions()
    {
        _gameSaves = SaveService.GetAllSaves();
        List<SaveGameOption> options = new(3);
        for (int i = 0; i < 3; i++)
        {
            var option = _saveGameOptionScene.Instantiate<SaveGameOption>();
            option.GameNameText = $"File {i + 1}";
            if (i < _gameSaves.Count)
                option.UpdateDisplay(_gameSaves[i].Item2, _gameSaves[i].Item1);
            options.Add(option);
        }
        return options;
    }
}
