using System.Collections.Generic;
using System.Linq;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class InventorySubMenu : OptionSubMenu
{
    public InventorySubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _inventory = gameSession?.MainParty?.Inventory ?? new Inventory();
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly AInventory _inventory;
    private readonly AItemCategoryDB _itemCategoryDB = Locator.ItemCategoryDB;
    private OptionContainer _inventoryList = null!;
    private DynamicTextContainer _itemInfo = null!;
    private ItemStatsDisplay _itemStatsDisplay = null!;
    private OptionContainer _typeList = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    private PackedScene _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && CurrentContainer == _inventoryList)
        {
            UpdateItemDescription(null);
            Audio.PlaySoundFX("menu_close1.wav");
            FocusContainer(_typeList);
            return;
        }
        base.HandleInput(menuInput, delta);
    }

    protected override void SetupOptions()
    {
        List<TextOption> typeOptions = GetItemTypeOptions();
        _typeList.ReplaceChildren(typeOptions);
        _typeList.FocusItem(1);
        _inventoryList.Clear();
        UpdateItemDescription(null);
    }

    protected override void OnItemFocused()
    {
        if (CurrentContainer == null)
            return;
        if (CurrentContainer == _typeList)
            UpdateItemList(CurrentContainer.FocusedItem, resetFocus: true);
        else
            UpdateItemDescription(CurrentContainer.FocusedItem);
    }

    protected override void OnItemSelected()
    {
        if (CurrentContainer == _typeList)
            FocusContainer(_inventoryList);
        else if (CurrentContainer == _inventoryList && CurrentContainer.FocusedItem != null)
            OpenUseSubMenu(CurrentContainer.FocusedItem);
    }

    public override void ResumeSubMenu()
    {
        UpdateItemList(_typeList.FocusedItem, resetFocus: false);
        base.ResumeSubMenu();
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _typeList = OptionContainers.First(x => x.Name == "ItemTypeOptions");
        _inventoryList = OptionContainers.First(x => x.Name == "InventoryOptions");
        _itemInfo = Foreground.GetNode<DynamicTextContainer>("ItemInfo");
        _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
    }

    private List<TextOption> GetItemTypeOptions()
    {
        List<TextOption> options = new(_itemCategoryDB.Categories.Count + 1);
        var allOption = _textOptionScene.Instantiate<TextOption>();
        allOption.LabelText = "All";
        allOption.OptionData[nameof(Item.ItemCategory.Id)] = "all";
        options.Add(allOption);
        foreach (ItemCategory category in _itemCategoryDB.Categories)
        {
            var option = _textOptionScene.Instantiate<TextOption>();
            option.LabelText = category.DisplayName;
            option.OptionData[nameof(Item.ItemCategory.Id)] = category.Id;
            options.Add(option);
        }
        return options;
    }

    private IEnumerable<KeyValueOption> GetItemOptions(string itemCategoryId)
    {
        IReadOnlyCollection<ItemStack> itemStacks;
        if (itemCategoryId == "all")
            itemStacks = _inventory.Items;
        else
            itemStacks = _inventory.GetItemsByType(itemCategoryId);
        if (!itemStacks.Any())
            return Enumerable.Empty<KeyValueOption>();
        List<KeyValueOption> options = new(itemStacks.Count);
        foreach (ItemStack itemStack in itemStacks)
        {
            var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
            option.KeyText = itemStack.Item.DisplayName;
            option.ValueText = "x" + itemStack.Count.ToString();
            option.OptionData[nameof(ItemStack)] = itemStack;
            options.Add(option);
        }
        return options;
    }

    private void OpenUseSubMenu(OptionItem optionItem)
    {
        if (!optionItem.TryGetData(nameof(ItemStack), out ItemStack? itemStack))
            return;
        _ = OpenSubMenuAsync(path: UseSubMenu.GetScenePath(), data: itemStack);
    }

    private void UpdateItemDescription(OptionItem? optionItem)
    {
        if (optionItem == null)
        {
            _itemStatsDisplay.UpdateStatsDisplay(null);
            _ = _itemInfo.UpdateTextAsync(string.Empty);
            _itemInfo.WriteTextEnabled = true;
        }
        else
        {
            if (!optionItem.TryGetData(nameof(ItemStack), out ItemStack? itemStack))
                return;
            _itemStatsDisplay.UpdateStatsDisplay(itemStack.Item);
            _ = _itemInfo.UpdateTextAsync(itemStack.Item.Description);
            _itemInfo.WriteTextEnabled = true;
        }
    }

    private void UpdateItemList(OptionItem optionItem, bool resetFocus)
    {
        if (optionItem == null)
            return;
        if (resetFocus)
            _inventoryList.ResetContainerFocus();
        if (!optionItem.TryGetData(nameof(Item.ItemCategory.Id), out string? itemCategoryId))
            return;
        IEnumerable<KeyValueOption> options = GetItemOptions(itemCategoryId);
        _inventoryList.ReplaceChildren(options);
    }
}
