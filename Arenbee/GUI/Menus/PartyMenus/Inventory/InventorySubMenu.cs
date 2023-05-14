using System.Collections.Generic;
using System.Linq;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using GameCore;
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
    private int _contentMargin;
    private AInventory _inventory;
    private readonly IItemCategoryDB _itemCategoryDB = ItemsLocator.ItemCategoryDB;
    private OptionContainer _inventoryList = null!;
    private DynamicTextBox _itemInfo = null!;
    private ItemStatsDisplay _itemStatsDisplay = null!;
    private Control _referenceContainer = null!;
    private OptionContainer _typeList = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    private PackedScene _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());

    protected override void OnMockPreSetup()
    {
        AItem potion = ItemsLocator.ItemDB.GetItem(ItemIds.Potion)!;
        AItem metalStick = ItemsLocator.ItemDB.GetItem(ItemIds.GeneSupreme)!;
        _inventory = new Inventory(new ItemStack[] { new(potion, 2), new(metalStick, 1) });
    }

    protected override void OnPreSetup(object? data)
    {
        if (data is not int margin)
            return;
        GetNode<MarginContainer>("%MarginContainer").SetLeftMargin(margin);
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && CurrentContainer == _inventoryList)
        {
            Audio.PlaySoundFX("menu_close1.wav");
            FocusContainer(_typeList);
            return;
        }
        base.HandleInput(menuInput, delta);
    }

    protected override void OnSetup()
    {
        SetNodeReferences();
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        _referenceContainer.Resized += OnResized;
        List<TextOption> typeOptions = GetItemTypeOptions();
        _typeList.ReplaceChildren(typeOptions);
        _typeList.FocusItem(1);
        _inventoryList.ClearOptionItems();
        UpdateItemDescription(null);
    }

    protected void OnResized()
    {
        _contentMargin = (int)(_referenceContainer.Position.X + _referenceContainer.Size.X);
    }

    protected override void OnContainerFocused(OptionContainer optionContainer)
    {
        if (optionContainer == _typeList)
            UpdateItemDescription(null);
    }

    protected override void OnItemFocused(OptionContainer optionContainer, OptionItem? optionItem)
    {
        if (optionItem == null)
            return;
        if (optionContainer == _typeList)
            UpdateItemList(optionItem, resetFocus: true);
        else
            UpdateItemDescription(optionItem);
    }

    protected override void OnSelectPressed()
    {
        if (CurrentContainer == _typeList)
            FocusContainer(_inventoryList);
        else if (CurrentContainer == _inventoryList && CurrentContainer.FocusedItem != null)
            OpenUseSubMenu(CurrentContainer.FocusedItem);
    }

    protected override void OnSubMenuResumed()
    {
        if (_typeList.FocusedItem != null)
            UpdateItemList(_typeList.FocusedItem, resetFocus: false);
    }

    private void SetNodeReferences()
    {
        _referenceContainer = GetNode<Control>("%VBoxContainer");
        _typeList = GetNode<OptionContainer>("%ItemTypeOptions");
        _inventoryList = GetNode<OptionContainer>("%InventoryOptions");
        AddContainer(_typeList);
        AddContainer(_inventoryList);
        _itemInfo = GetNode<DynamicTextBox>("%ItemInfo");
        _itemStatsDisplay = GetNode<ItemStatsDisplay>("%ItemStatsDisplay");
    }

    private List<TextOption> GetItemTypeOptions()
    {
        IReadOnlyCollection<ItemCategory> categories = _itemCategoryDB.GetCategories();
        List<TextOption> options = new(categories.Count + 1);
        var allOption = _textOptionScene.Instantiate<TextOption>();
        allOption.LabelText = "All";
        allOption.OptionData = "all";
        options.Add(allOption);
        foreach (ItemCategory category in categories)
        {
            var option = _textOptionScene.Instantiate<TextOption>();
            option.LabelText = category.DisplayName;
            option.OptionData = category.Id;
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
            option.OptionData = itemStack;
            options.Add(option);
        }
        return options;
    }

    private void OpenUseSubMenu(OptionItem optionItem)
    {
        if (optionItem.OptionData is not ItemStack itemStack)
            return;
        _ = OpenSubMenuAsync(path: UseSubMenu.GetScenePath(), data: (_contentMargin, itemStack));
    }

    private void UpdateItemDescription(OptionItem? optionItem)
    {
        if (optionItem == null)
        {
            _itemStatsDisplay.UpdateStatsDisplay(null);
            _itemInfo.CustomText = string.Empty;
            _itemInfo.Writing = true;
        }
        else
        {
            if (optionItem.OptionData is not ItemStack itemStack)
                return;
            _itemStatsDisplay.UpdateStatsDisplay(itemStack.Item);
            _ = _itemInfo.CustomText = itemStack.Item.Description;
            _itemInfo.Writing = true;
        }
    }

    private void UpdateItemList(OptionItem optionItem, bool resetFocus)
    {
        if (resetFocus)
            _inventoryList.ResetContainerFocus();
        if (optionItem?.OptionData is not string itemCategoryId)
            return;
        IEnumerable<KeyValueOption> options = GetItemOptions(itemCategoryId);
        _inventoryList.ReplaceChildren(options);
    }
}
