using System.Collections.Generic;
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
            UpdateItemDescription(null);
            Locator.Audio.PlaySoundFX("menu_close1.wav");
            FocusContainer(_typeList);
            return;
        }
        base.HandleInput(menuInput, delta);
    }

    protected override void ReplaceDefaultOptions()
    {
        var typeOptions = GetItemTypeOptions();
        _typeList.GridContainer.Columns = typeOptions.Count;
        _typeList.ReplaceChildren(typeOptions);
        _typeList.CurrentIndex = 1;
        _inventoryList.Clear();
        UpdateItemDescription(null);
    }

    protected override void OnItemFocused()
    {
        base.OnItemFocused();
        if (CurrentContainer == _typeList)
            UpdateItemList(CurrentContainer.CurrentItem, resetFocus: true);
        else
            UpdateItemDescription(CurrentContainer.CurrentItem);
    }

    protected override void OnItemSelected()
    {
        base.OnItemSelected();
        if (CurrentContainer == _typeList)
            FocusContainer(_inventoryList);
        else if (CurrentContainer == _inventoryList)
            OpenUseSubMenu(CurrentContainer.CurrentItem);
    }

    public override void ResumeSubMenu()
    {
        UpdateItemList(_typeList.CurrentItem, resetFocus: false);
        base.ResumeSubMenu();
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _typeList = OptionContainers.Find(x => x.Name == "TypeList");
        _inventoryList = OptionContainers.Find(x => x.Name == "InventoryList");
        _itemInfo = Foreground.GetNode<DynamicTextContainer>("ItemInfo");
        _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
        _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
        _inventory = Locator.GetParty()?.Inventory;
    }

    private List<TextOption> GetItemTypeOptions()
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
        string itemId = optionItem.GetData<string>(nameof(ItemStack.ItemId));
        ItemStack itemStack = _inventory.GetItemStack(itemId);
        if (itemStack == null)
            return;
        var useSubMenu = GDEx.Instantiate<UseSubMenu>(UseSubMenu.GetScenePath());
        useSubMenu.ItemStack = itemStack;
        RaiseRequestedAdd(useSubMenu);
    }

    private void UpdateItemDescription(OptionItem optionItem)
    {
        string itemId = optionItem?.GetData<string>(nameof(ItemStack.ItemId));
        ItemBase item = Locator.ItemDB.GetItem(itemId);
        _itemStatsDisplay.UpdateStatsDisplay(item);
        _itemInfo.UpdateText(item?.Description);
    }

    private void UpdateItemList(OptionItem optionItem, bool resetFocus)
    {
        if (optionItem == null)
            return;
        if (resetFocus)
            _inventoryList.ResetContainerFocus();
        string itemCategoryId = optionItem.GetData<string>(nameof(Item.ItemCategoryId));
        List<KeyValueOption> options = GetItemOptions(itemCategoryId);
        _inventoryList.ReplaceChildren(options);
    }
}
