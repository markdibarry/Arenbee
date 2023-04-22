using System.Collections.Generic;
using System.Linq;
using Arenbee.SaveData;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveConfirmSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private string? _fileName;
    private GameSession? _gameSession = Locator.Session as GameSession;
    private readonly List<string> _menuKeys = new()
    {
        Localization.Menus.Menus_SaveConfirm_Yes,
        Localization.Menus.Menus_SaveConfirm_No
    };

    public override void SetupData(object? data)
    {
        if (data is not string fileName)
            return;
        _fileName = fileName;
    }

    protected override void SetupOptions()
    {
        OptionContainer? saveOptions = OptionContainers.First(x => x.Name == "SaveOptions");
        saveOptions.ReplaceChildren(GetMenuOptions());
    }

    protected override void OnItemSelected()
    {
        if (CurrentContainer?.FocusedItem?.OptionData is not string saveChoice)
            return;
        switch (saveChoice)
        {
            case Localization.Menus.Menus_SaveConfirm_Yes:
                SaveGame();
                break;
            case Localization.Menus.Menus_SaveConfirm_No:
                _ = CloseSubMenuAsync();
                break;
        }
    }

    private void SaveGame()
    {
        if (_gameSession == null)
            return;
        SaveService.SaveGame(new GameSave(0, _gameSession), _fileName);
        _ = OpenSubMenuAsync(SaveSuccessSubMenu.GetScenePath());
    }

    private List<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        List<TextOption> options = new();
        foreach (string menuKey in _menuKeys)
        {
            TextOption option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = Tr(menuKey);
            option.OptionData = menuKey;
            options.Add(option);
        }
        return options;
    }
}
