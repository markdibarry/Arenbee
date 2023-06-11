using System.Collections.Generic;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveGameSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private OptionContainer _saveOptions = null!;
    private PackedScene _saveGameOptionScene = GD.Load<PackedScene>(SaveGameOption.GetScenePath());

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem? optionItem)
    {
        string? fileName = optionItem?.OptionData as string;
        OpenSaveGameConfirmSubMenu(fileName);
    }

    protected override void OnSetup()
    {
        SetNodeReferences();
        var header = GetNode<Label>("%Header");
        header.Text = this.TrS(Localization.Menus.Menus_Save_SavedGames);

        UpdateSaveOptions();
    }

    protected override void OnSubMenuResumed()
    {
        UpdateSaveOptions();
    }

    private List<SaveGameOption> GetSaveGameOptions()
    {
        List<(string, GameSave)> gameSaves = SaveService.GetAllSaves();
        List<SaveGameOption> options = new(3);
        for (int i = 0; i < 3; i++)
        {
            var option = _saveGameOptionScene.Instantiate<SaveGameOption>();
            option.GameNameText = $"File {i + 1}";
            if (i < gameSaves.Count)
                option.UpdateDisplay(gameSaves[i].Item2, gameSaves[i].Item1);
            options.Add(option);
        }
        return options;
    }

    private void OpenSaveGameConfirmSubMenu(string? fileName)
    {
        _ = OpenSubMenuAsync(path: SaveConfirmSubMenu.GetScenePath(), data: fileName);
    }

    private void SetNodeReferences()
    {
        _saveOptions = GetNode<OptionContainer>("%SaveOptions");
        AddContainer(_saveOptions);
    }

    private void UpdateSaveOptions()
    {
        List<SaveGameOption> options = GetSaveGameOptions();
        _saveOptions?.ReplaceChildren(options);
    }
}
