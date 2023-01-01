﻿using System.Collections.Generic;
using Arenbee.GUI.Menus.Common;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Party.Equipment;

[Tool]
public partial class SelectSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private string _currentItemId;
    private OptionContainer _equipOptions;
    private ItemDBBase _itemDB;
    private PackedScene _keyValueOptionScene;
    private Stats _mockStats;
    private PlayerParty _playerParty;
    private ActorStatsDisplay _actorStatsDisplay;
    private ItemStatsDisplay _itemStatsDisplay;
    public ActorBase Actor { get; set; }
    public EquipmentSlotBase Slot { get; set; }

    public override void SetupData(object data)
    {
        if (data is not SelectSubMenuDataModel dataModel)
            return;
        Actor = dataModel.Actor;
        Slot = dataModel.Slot;
    }

    protected override void SetupOptions()
    {
        var options = GetEquippableOptions();
        _equipOptions.ReplaceChildren(options);
        _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
        _itemStatsDisplay.UpdateStatsDisplay(null);
    }

    protected override void OnItemFocused()
    {
        if (CurrentContainer.FocusedItem == null)
        {
            _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
            _itemStatsDisplay.UpdateStatsDisplay(null);
            return;
        }
        _itemDB.GetItem(_currentItemId)?.RemoveFromStats(_mockStats);
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(ItemStack.ItemId), out _currentItemId))
            return;
        var newItem = _itemDB.GetItem(_currentItemId);
        newItem?.AddToStats(_mockStats);
        _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
        _itemStatsDisplay.UpdateStatsDisplay(newItem);
    }

    protected override void OnItemSelected()
    {
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(ItemStack.ItemId), out string? itemId))
            return;
        if (TryEquip(itemId, Slot))
        {
            CloseSoundPath = string.Empty;
            _ = CloseSubMenuAsync();
        }
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _itemDB = Locator.ItemDB;
        _mockStats = Actor?.Stats == null ? null : new Stats(Actor.Stats);
        _currentItemId = Slot?.ItemId;
        _equipOptions = OptionContainers.Find(x => x.Name == "EquipOptions");
        _actorStatsDisplay = Foreground.GetNode<ActorStatsDisplay>("ActorStatsDisplay");
        _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
        _playerParty = Locator.GetParty() ?? new PlayerParty();
        _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
    }

    private bool TryEquip(string itemId, EquipmentSlotBase slot)
    {
        return Actor.Equipment.TrySetItemById(slot, itemId);
    }

    private List<KeyValueOption> GetEquippableOptions()
    {
        var options = new List<KeyValueOption>();
        var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
        unequipOption.KeyText = "<Unequip>";
        unequipOption.ValueText = string.Empty;
        unequipOption.OptionData[nameof(ItemStack.ItemId)] = "";
        options.Add(unequipOption);
        if (Slot == null)
            return options;
        foreach (var itemStack in _playerParty.Inventory?.GetItemsByType(Slot.SlotCategory.ItemCategoryId))
        {
            var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
            option.KeyText = itemStack.Item.DisplayName;
            option.ValueText = "x" + itemStack.Amount.ToString();
            option.OptionData[nameof(ItemStack.ItemId)] = itemStack.ItemId;
            if (!itemStack.CanReserve())
                option.Disabled = true;
            if (itemStack.ItemId == Slot.ItemId)
                _equipOptions.FocusItem(options.Count);
            options.Add(option);
        }
        return options;
    }
}
