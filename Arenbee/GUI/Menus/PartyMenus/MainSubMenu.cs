﻿using System.Collections.Generic;
using Arenbee.GUI.Menus.Common;
using Arenbee.GUI.Menus.PartyMenus.Equipment;
using GameCore;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private GameRoot _gameRoot = (GameRoot)Locator.Root;
    private Control _referenceContainer = null!;
    private OptionContainer _mainOptions = null!;
    private int _contentMargin;
    private readonly List<string> _menuKeys = new()
    {
        Localization.Menus.Menus_Party_Stats,
        Localization.Menus.Menus_Party_Inventory,
        Localization.Menus.Menus_Party_Equipment,
        Localization.Menus.Menus_Party_Options,
        Localization.Menus.Menus_Party_Save,
        Localization.Menus.Menus_Party_Quit
    };

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem? optionItem)
    {
        if (optionItem?.OptionData is not string subMenuName)
            return;

        switch (subMenuName)
        {
            case Localization.Menus.Menus_Party_Stats:
                _ = OpenSubMenuAsync(StatsSubMenu.GetScenePath(), data: _contentMargin);
                break;
            case Localization.Menus.Menus_Party_Inventory:
                _ = OpenSubMenuAsync(InventorySubMenu.GetScenePath(), data: _contentMargin);
                break;
            case Localization.Menus.Menus_Party_Equipment:
                _ = OpenSubMenuAsync(EquipmentSubMenu.GetScenePath(), data: _contentMargin);
                break;
            case Localization.Menus.Menus_Party_Save:
                _ = OpenSubMenuAsync(SaveGameSubMenu.GetScenePath());
                break;
            case Localization.Menus.Menus_Party_Quit:
                QuitToTitle();
                break;
        }
    }

    protected override void OnSetup()
    {
        SetNodeReferences();
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        _referenceContainer.ItemRectChanged += OnRefRectChanged;
        _mainOptions.ReplaceChildren(GetMenuOptions());
    }

    protected void OnRefRectChanged()
    {
        _contentMargin = (int)(_referenceContainer.Position.X + _referenceContainer.Size.X);
    }

    private void SetNodeReferences()
    {
        _referenceContainer = GetNode<Control>("%MainContainer");
        _mainOptions = GetNode<OptionContainer>("%MainOptions");
        AddContainer(_mainOptions);
    }

    private IEnumerable<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        List<TextOption> options = new();
        foreach (string menuKey in _menuKeys)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = this.TrS(menuKey);
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
