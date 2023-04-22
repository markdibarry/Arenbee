using System.Collections.Generic;
using System.Linq;
using Arenbee.GUI.Menus.Common;
using Arenbee.GUI.Menus.PartyMenus.Equipment;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private GameRoot _gameRoot = (GameRoot)Locator.Root;
    private OptionContainer _optionList = null!;
    private readonly List<string> _menuKeys = new()
    {
        Localization.Menus.Menus_Party_Stats,
        Localization.Menus.Menus_Party_Inventory,
        Localization.Menus.Menus_Party_Equipment,
        Localization.Menus.Menus_Party_Options,
        Localization.Menus.Menus_Party_Save,
        Localization.Menus.Menus_Party_Quit
    };

    protected override void OnItemSelected()
    {
        if (CurrentContainer?.FocusedItem?.OptionData is not string subMenuName)
            return;

        switch (subMenuName)
        {
            case Localization.Menus.Menus_Party_Stats:
                _ = OpenSubMenuAsync(StatsSubMenu.GetScenePath());
                break;
            case Localization.Menus.Menus_Party_Inventory:
                _ = OpenSubMenuAsync(InventorySubMenu.GetScenePath());
                break;
            case Localization.Menus.Menus_Party_Equipment:
                _ = OpenSubMenuAsync(EquipmentSubMenu.GetScenePath());
                break;
            case Localization.Menus.Menus_Party_Save:
                _ = OpenSubMenuAsync(SaveGameSubMenu.GetScenePath());
                break;
            case Localization.Menus.Menus_Party_Quit:
                QuitToTitle();
                break;
        }
    }

    protected override void SetupOptions()
    {
        _optionList.ReplaceChildren(GetMenuOptions());
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _optionList = OptionContainers.First(x => x.Name == "OptionContainer");
    }

    private IEnumerable<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        List<TextOption> options = new();
        foreach (string menuKey in _menuKeys)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = Tr(menuKey);
            option.OptionData = menuKey;
            if (menuKey == Localization.Menus.Menus_Party_Options)
                option.Disabled = true;
            options.Add(option);
        }
        return options;
    }

    private void QuitToTitle()
    {
        CurrentState = State.Busy;
        _gameRoot.ResetToTitleScreen(
            BasicLoadingScreen.GetScenePath(),
            WipeTransition.GetScenePath(),
            FadeTransition.GetScenePath());
    }
}
