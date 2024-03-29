﻿using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors;
using GameCore.Actors;
using GameCore;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus.Equipment;

[Tool]
public partial class EquipmentSubMenu : OptionSubMenu
{
    public EquipmentSubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _partyActors = gameSession?.MainParty?.Actors ?? Array.Empty<Actor>();
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private Control _referenceContainer = null!;
    private int _contentMargin;
    private GridOptionContainer _partyOptions = null!;
    private OptionContainer _equipmentOptions = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    private PackedScene _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
    private IReadOnlyCollection<Actor> _partyActors;

    protected override void OnPreSetup(object? data)
    {
        if (data is not int margin)
            return;
        GetNode<MarginContainer>("%MarginContainer").SetLeftMargin(margin);
    }

    protected override void OnMockPreSetup()
    {
        Actor actor = ActorsLocator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)?.ToActor()!;
        _partyActors = new List<Actor> { actor };
    }

    protected override void OnSetup()
    {
        SetNodeReferences();
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        _referenceContainer.ItemRectChanged += OnRefRectChanged;
        UpdatePartyMemberOptions();
    }

    public override void HandleInput(IGUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && CurrentContainer == _equipmentOptions)
            FocusContainer(_partyOptions);
        else
            base.HandleInput(menuInput, delta);
    }

    protected override void OnSubMenuResumed()
    {
        UpdateCurrentEquipmentOption(_equipmentOptions.FocusedItem);
    }

    protected void OnRefRectChanged()
    {
        _contentMargin = (int)(_referenceContainer.Position.X + _referenceContainer.Size.X);
    }

    protected override void OnItemFocused(OptionContainer optionContainer, OptionItem? optionItem)
    {
        if (optionContainer == _partyOptions)
            UpdateEquipmentDisplay(optionItem);
    }

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem? optionItem)
    {
        if (optionContainer == _partyOptions)
            FocusContainer(_equipmentOptions);
        else
            OpenEquipSelectMenu(optionItem);
    }

    private void SetNodeReferences()
    {
        _referenceContainer = GetNode<Control>("%VBoxContainer");
        _partyOptions = GetNode<GridOptionContainer>("%PartyOptions");
        _equipmentOptions = GetNode<OptionContainer>("%EquipmentOptions");
        AddContainer(_partyOptions);
        AddContainer(_equipmentOptions);
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        List<TextOption> options = new();
        if (_partyActors.Count == 0)
            return options;
        _partyOptions.Columns = _partyActors.Count;
        foreach (Actor actorData in _partyActors.OfType<Actor>())
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData = actorData;
            textOption.LabelText = actorData.Name;
            options.Add(textOption);
        }
        return options;
    }

    private void OpenEquipSelectMenu(OptionItem? optionItem)
    {
        if (_partyOptions.FocusedItem?.OptionData is not Actor actor)
            return;
        if (optionItem?.OptionData is not EquipmentSlot slot)
            return;

        SelectSubMenuDataModel data = new(slot, actor, _contentMargin);
        _ = OpenSubMenuAsync(path: SelectSubMenu.GetScenePath(), data: data);
    }

    private void UpdateEquipmentDisplay(OptionItem? optionItem)
    {
        if (optionItem?.OptionData is not Actor actor)
            return;
        _equipmentOptions.ClearOptionItems();
        foreach (EquipmentSlot slot in actor.Equipment.Slots)
        {
            var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
            option.KeyText = slot.SlotCategory.Abbreviation + ":";
            option.ValueText = slot.ItemStack?.Item.DisplayName ?? "<None>";
            option.OptionData = slot;
            _equipmentOptions.AddOption(option);
        }
    }

    private static void UpdateCurrentEquipmentOption(OptionItem? optionItem)
    {
        if (optionItem?.OptionData is not EquipmentSlot slot)
            return;
        if (optionItem is not KeyValueOption kvOption)
            return;
        kvOption.ValueText = slot.ItemStack?.Item.DisplayName ?? "<None>";
    }

    private void UpdatePartyMemberOptions()
    {
        List<TextOption> options = GetPartyMemberOptions();
        _partyOptions.ReplaceChildren(options);
    }
}
