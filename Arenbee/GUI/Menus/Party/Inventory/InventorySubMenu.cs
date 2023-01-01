using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Party;

[Tool]
public partial class InventorySubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private Inventory _inventory;
    private OptionContainer _inventoryList;
    private DynamicTextContainer _itemInfo;
    private ItemStatsDisplay _itemStatsDisplay;
    private OptionContainer _typeList;
    private PackedScene _keyValueOptionScene;

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && CurrentContainer == _inventoryList)
        {
            _ = UpdateItemDescription(null);
            Locator.Audio.PlaySoundFX("menu_close1.wav");
            FocusContainer(_typeList);
            return;
        }
        base.HandleInput(menuInput, delta);
    }

    protected override void SetupOptions()
    {
        var typeOptions = GetItemTypeOptions();
        _typeList.ReplaceChildren(typeOptions);
        _typeList.FocusItem(1);
        _inventoryList.Clear();
        _ = UpdateItemDescription(null);
    }

    protected override void OnItemFocused()
    {
        if (CurrentContainer == _typeList)
            UpdateItemList(CurrentContainer.FocusedItem, resetFocus: true);
        else
            _ = UpdateItemDescription(CurrentContainer.FocusedItem);
    }

    protected override void OnItemSelected()
    {
        if (CurrentContainer == _typeList)
            FocusContainer(_inventoryList);
        else if (CurrentContainer == _inventoryList)
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
        _typeList = OptionContainers.Find(x => x.Name == "ItemTypeOptions");
        _inventoryList = OptionContainers.Find(x => x.Name == "InventoryOptions");
        _itemInfo = Foreground.GetNode<DynamicTextContainer>("ItemInfo");
        _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
        _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
        _inventory = Locator.GetParty()?.Inventory;
    }

    private static List<TextOption> GetItemTypeOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        var itemCategories = Locator.ItemCategoryDB.Categories;
        var allOption = textOptionScene.Instantiate<TextOption>();
        allOption.LabelText = "All";
        allOption.OptionData[nameof(Item.ItemCategoryId)] = "all";
        options.Add(allOption);
        foreach (var category in itemCategories)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = category.Name;
            option.OptionData[nameof(Item.ItemCategoryId)] = category.Id;
            options.Add(option);
        }
        return options;
    }

    private List<KeyValueOption> GetItemOptions(string itemCategoryId)
    {
        var options = new List<KeyValueOption>();
        ICollection<ItemStack> itemStacks;
        if (itemCategoryId == "all")
            itemStacks = _inventory?.Items;
        else
            itemStacks = _inventory?.GetItemsByType(itemCategoryId);
        if (itemStacks == null)
            return options;
        foreach (var itemStack in itemStacks)
        {
            var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
            option.KeyText = itemStack.Item.DisplayName;
            option.ValueText = "x" + itemStack.Amount.ToString();
            option.OptionData[nameof(ItemStack.ItemId)] = itemStack.ItemId;
            options.Add(option);
        }
        return options;
    }

    private void OpenUseSubMenu(OptionItem optionItem)
    {
        if (!optionItem.TryGetData(nameof(ItemStack.ItemId), out string? itemId))
            return;
        ItemStack itemStack = _inventory.GetItemStack(itemId);
        if (itemStack == null)
            return;
        _ = OpenSubMenuAsync(path: UseSubMenu.GetScenePath(), data: itemStack);
    }

    private async Task UpdateItemDescription(OptionItem optionItem)
    {
        if (!optionItem.TryGetData(nameof(ItemStack.ItemId), out string? itemId))
            return;
        ItemBase item = Locator.ItemDB.GetItem(itemId);
        _itemStatsDisplay.UpdateStatsDisplay(item);
        await _itemInfo.UpdateTextAsync(item?.Description);
        _itemInfo.WriteTextEnabled = true;
    }

    private void UpdateItemList(OptionItem optionItem, bool resetFocus)
    {
        if (optionItem == null)
            return;
        if (resetFocus)
            _inventoryList.ResetContainerFocus();
        if (!optionItem.TryGetData(nameof(Item.ItemCategoryId), out string? itemCategoryId))
            return;
        List<KeyValueOption> options = GetItemOptions(itemCategoryId);
        _inventoryList.ReplaceChildren(options);
    }
}
