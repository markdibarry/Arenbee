using System.Collections.Generic;
using System.Linq;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.SaveData;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveConfirmSubMenu : OptionSubMenu
{
    private int _gameSaveId;
    private readonly List<string> _menuKeys = new()
    {
        Localization.Menus.Menus_SaveConfirm_Yes,
        Localization.Menus.Menus_SaveConfirm_No
    };
    public static string GetScenePath() => GDEx.GetScenePath();

    public override void SetupData(object? data)
    {
        if (data is not int gameSaveId)
            return;
        _gameSaveId = gameSaveId;
    }

    protected override void SetupOptions()
    {
        OptionContainer? saveOptions = OptionContainers.First(x => x.Name == "SaveOptions");
        saveOptions.ReplaceChildren(GetMenuOptions());
    }

    protected override void OnItemSelected()
    {
        if (!CurrentContainer.FocusedItem.TryGetData("value", out string? saveChoice))
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
        SaveService.SaveGame(_gameSaveId, Locator.Session);
        _ = OpenSubMenuAsync(SaveSuccessSubMenu.GetScenePath());
    }

    private List<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        foreach (var menuKey in _menuKeys)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = Tr(menuKey);
            option.OptionData["value"] = menuKey;
            options.Add(option);
        }
        return options;
    }
}
