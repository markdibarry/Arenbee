using System.Collections.Generic;
using Arenbee.Actors;
using Arenbee.Game;
using Arenbee.GUI.Menus.Common;
using Arenbee.Statistics;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus.Equipment;

[Tool]
public partial class SelectSubMenu : OptionSubMenu
{
    public SelectSubMenu()
    {
        _itemDB = Locator.ItemDB;
        GameSession? gameSession = Locator.Session as GameSession;
        _playerParty = gameSession?.MainParty ?? new Party("temp");
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly AItemDB _itemDB;
    private readonly Party _playerParty;
    private Actor _actor = null!;
    private EquipmentSlot _slot = null!;
    private ItemStack? _currentItemStack;
    private OptionContainer _equipOptions = null!;
    private PackedScene _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
    private Stats _mockStats = null!;
    private readonly List<Modifier> _mockMods = new();
    private ActorStatsDisplay _actorStatsDisplay = null!;
    private ItemStatsDisplay _itemStatsDisplay = null!;

    public override void SetupData(object? data)
    {
        if (data is not SelectSubMenuDataModel dataModel)
            return;
        _actor = dataModel.Actor;
        _slot = dataModel.Slot;
    }

    protected override void SetupOptions()
    {
        var options = GetEquippableOptions();
        _equipOptions.ReplaceChildren(options);
        _actorStatsDisplay.UpdateBaseValues((Stats)_actor.Stats);
    }

    protected override void OnItemFocused()
    {
        if (_currentItemStack != null)
        {
            foreach (Modifier mod in _mockMods)
                _mockStats.RemoveMod(mod, false);
        }

        _mockMods.Clear();

        if (CurrentContainer.FocusedItem.TryGetData(nameof(ItemStack), out _currentItemStack))
        {
            foreach (Modifier mod in _currentItemStack.Item.Modifiers)
            {
                Modifier newMod = new(mod);
                _mockMods.Add(newMod);
                _mockStats.AddMod(newMod);
            }
        }

        _actorStatsDisplay.UpdateStatsDisplay(_mockStats, updateColor: true);
        _itemStatsDisplay.UpdateStatsDisplay(_currentItemStack?.Item);
    }

    protected override void OnItemSelected()
    {
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(ItemStack), out ItemStack? itemStack))
            _actor.Equipment.RemoveItem(_actor, _slot);
        else
            _actor.Equipment.TrySetItem(_actor, _slot, itemStack);
        CloseSoundPath = string.Empty;
        _ = CloseSubMenuAsync();
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        CreateMockStats();
        _currentItemStack = _slot.ItemStack;
        _equipOptions = OptionContainers.Find(x => x.Name == "EquipOptions");
        _actorStatsDisplay = Foreground.GetNode<ActorStatsDisplay>("ActorStatsDisplay");
        _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
    }

    private void CreateMockStats()
    {
        _mockStats = new((Stats)_actor.Stats);
        foreach (var mod in _slot.Modifiers)
            _mockStats.RemoveMod(mod, false);
    }

    private bool TryEquip(ItemStack itemStack, EquipmentSlot slot)
    {
        return _actor.Equipment.TrySetItem(_actor, slot, itemStack);
    }

    private List<KeyValueOption> GetEquippableOptions()
    {
        List<KeyValueOption> options = new();
        var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
        unequipOption.KeyText = "<Unequip>";
        unequipOption.ValueText = string.Empty;
        unequipOption.OptionData[nameof(ItemStack)] = null;
        options.Add(unequipOption);
        if (_slot == null)
            return options;
        if (_playerParty.Inventory == null)
            return options;
        foreach (string itemCategoryId in _slot.SlotCategory.ItemCategoryIds)
        {
            foreach (ItemStack itemStack in _playerParty.Inventory.GetItemsByType(itemCategoryId))
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Count.ToString();
                option.OptionData[nameof(ItemStack)] = itemStack;
                if (!itemStack.CanReserve())
                    option.Disabled = true;
                if (itemStack.Item == _slot.Item)
                    _equipOptions.FocusItem(options.Count);
                options.Add(option);
            }
        }

        return options;
    }
}
