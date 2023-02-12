using System.Collections.Generic;
using Arenbee.GUI.Menus.Common;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;
using Arenbee.Items;
using Arenbee.Game;

namespace Arenbee.GUI.Menus.Party.Equipment;

[Tool]
public partial class SelectSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private AItemStack? _currentItemStack;
    private OptionContainer _equipOptions;
    private AItemDB _itemDB;
    private PackedScene _keyValueOptionScene;
    private Stats _mockStats;
    private PlayerParty _playerParty;
    private ActorStatsDisplay _actorStatsDisplay;
    private ItemStatsDisplay _itemStatsDisplay;
    public AActor Actor { get; set; }
    public EquipmentSlot Slot { get; set; }

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
        _currentItemStack.Item.RemoveFromStats(_mockStats);
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(ItemStack), out _currentItemStack))
            return;
        _currentItemStack.Item.AddToStats(_mockStats);
        _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
        _itemStatsDisplay.UpdateStatsDisplay(_currentItemStack.Item);
    }

    protected override void OnItemSelected()
    {
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(ItemStack), out AItemStack? itemStack))
            return;
        if (TryEquip(itemStack, Slot))
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
        _currentItemStack = Slot?.ItemStack;
        _equipOptions = OptionContainers.Find(x => x.Name == "EquipOptions");
        _actorStatsDisplay = Foreground.GetNode<ActorStatsDisplay>("ActorStatsDisplay");
        _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
        GameSession? gameSession = Locator.Session as GameSession;
        _playerParty = gameSession?.Party ?? new PlayerParty();
        _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
    }

    private bool TryEquip(AItemStack itemStack, EquipmentSlot slot)
    {
        return Actor.Equipment.TrySetItem(Actor, slot, itemStack);
    }

    private List<KeyValueOption> GetEquippableOptions()
    {
        var options = new List<KeyValueOption>();
        var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
        unequipOption.KeyText = "<Unequip>";
        unequipOption.ValueText = string.Empty;
        unequipOption.OptionData[nameof(ItemStack)] = null;
        options.Add(unequipOption);
        if (Slot == null)
            return options;
        if (_playerParty.Inventory == null)
            return options;
        foreach (string itemCategoryId in Slot.SlotCategory.ItemCategoryIds)
        {
            foreach (AItemStack itemStack in _playerParty.Inventory.GetItemsByType(itemCategoryId))
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Count.ToString();
                option.OptionData[nameof(ItemStack)] = itemStack;
                if (!itemStack.CanReserve())
                    option.Disabled = true;
                if (itemStack.Item == Slot.Item)
                    _equipOptions.FocusItem(options.Count);
                options.Add(option);
            }
        }

        return options;
    }
}
