using System.Collections.Generic;
using Arenbee.Actors;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore;
using GameCore.Actors;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus.Equipment;

[Tool]
public partial class SelectSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private Inventory _inventory = null!;
    private Actor _actor = null!;
    private EquipmentSlot _slot = null!;
    private ItemStack? _currentItemStack;
    private OptionContainer _equipOptions = null!;
    private PackedScene _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
    private Stats _mockStats = null!;
    private readonly List<Modifier> _mockMods = new();
    private ActorStatsDisplay _actorStatsDisplay = null!;
    private ItemStatsDisplay _itemStatsDisplay = null!;

    protected override void OnPreSetup(object? data)
    {
        if (data is not SelectSubMenuDataModel dataModel)
            return;
        _actor = dataModel.Actor;
        _slot = dataModel.Slot;
        GetNode<MarginContainer>("%MarginContainer").SetLeftMargin(dataModel.Margin);
        GameSession? gameSession = Locator.Session as GameSession;
        _inventory = gameSession?.MainParty?.Inventory!;
    }

    protected override void OnMockPreSetup()
    {
        _actor = ActorsLocator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)?.CreateActor()!;
        EquipmentSlotCategory category = ItemsLocator.EquipmentSlotCategoryDB.GetCategory(EquipmentSlotCategoryIds.Weapon)!;
        ItemStack metalStick = new(ItemsLocator.ItemDB.GetItem(ItemIds.MetalHockeyStick)!, 1);
        ItemStack magicWand = new(ItemsLocator.ItemDB.GetItem(ItemIds.Wand)!, 1);
        _slot = new EquipmentSlot(category, metalStick);
        _inventory = new Inventory(new ItemStack[] { metalStick, magicWand });
    }

    protected override void OnSetup()
    {
        SetNodeReferences();
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        var options = GetEquippableOptions();
        _equipOptions.ReplaceChildren(options);
        _actorStatsDisplay.UpdateBaseValues(_actor.Stats);
    }

    protected override void OnItemFocused(OptionContainer optionContainer, OptionItem? optionItem)
    {
        RemoveMockMods();

        _currentItemStack = optionItem?.OptionData as ItemStack;
        if (_currentItemStack != null)
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

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem optionItem)
    {
        if (optionItem.OptionData is not ItemStack itemStack)
            _actor.Equipment.RemoveItem(_actor, _slot);
        else
            _actor.Equipment.TrySetItem(_actor, _slot, itemStack);
        CloseSoundPath = string.Empty;
        _ = CloseSubMenuAsync();
    }

    private void SetNodeReferences()
    {
        CreateMockStats();
        _currentItemStack = _slot.ItemStack;
        _equipOptions = GetNode<OptionContainer>("%EquipOptions");
        AddContainer(_equipOptions);
        _actorStatsDisplay = GetNode<ActorStatsDisplay>("%ActorStatsDisplay");
        _itemStatsDisplay = GetNode<ItemStatsDisplay>("%ItemStatsDisplay");
    }

    private void CreateMockStats()
    {
        _mockStats = new(_actor.Stats);
        RemoveMockMods();
    }

    private List<KeyValueOption> GetEquippableOptions()
    {
        List<KeyValueOption> options = new();
        var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
        unequipOption.KeyText = "<Unequip>";
        unequipOption.ValueText = string.Empty;
        unequipOption.OptionData = null;
        options.Add(unequipOption);
        if (_slot == null || _inventory == null)
            return options;
        foreach (string itemCategoryId in _slot.SlotCategory.ItemCategoryIds)
        {
            foreach (ItemStack itemStack in _inventory.GetItemsByType(itemCategoryId))
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Count.ToString();
                option.OptionData = itemStack;
                if (!itemStack.CanReserve())
                    option.Disabled = true;
                if (itemStack.Item == _slot.Item)
                    _equipOptions.FocusItem(options.Count);
                options.Add(option);
            }
        }

        return options;
    }

    private void RemoveMockMods()
    {
        if (_currentItemStack != null)
        {
            foreach (Modifier mod in _mockMods)
                _mockStats.RemoveMod(mod);
        }

        _mockMods.Clear();
    }
}
