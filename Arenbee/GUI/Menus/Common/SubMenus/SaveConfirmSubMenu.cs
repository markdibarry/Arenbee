using System.Collections.Generic;
using GameCore;
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

    protected override void OnPreSetup(object? data)
    {
        if (data is not string fileName)
            return;
        _fileName = fileName;
    }

    protected override void OnSetup()
    {
        var message = GetNode<Label>("%Message");
        message.Text = this.TrS(Localization.Menus.Menus_SaveConfirm_Message);
        OptionContainer? saveOptions = GetNode<OptionContainer>("%SaveOptions");
        AddContainer(saveOptions);
        saveOptions.ReplaceChildren(GetMenuOptions());
    }

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem optionItem)
    {
        if (optionItem.OptionData is not string saveChoice)
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
            option.LabelText = this.TrS(menuKey);
            option.OptionData = menuKey;
            options.Add(option);
        }
        return options;
    }
}
