using Arenbee.Items;
using GameCore;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class DropSubMenu : OptionSubMenu
{
    public DropSubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _inventory = gameSession?.MainParty?.Inventory ?? new Inventory();
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private ItemStack _itemStack = null!;
    private readonly Inventory _inventory;
    private PackedScene _numberOptionScene = GD.Load<PackedScene>(NumberOption.GetScenePath());
    private OptionContainer _optionContainer = null!;
    private NumberOption _numberOption = null!;
    private int _count = 1;

    protected override void OnSetup()
    {
        SetNodeReferences();
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        DisplayOptions();
    }

    private void SetNodeReferences()
    {
        var label = GetNode<Label>("%Message");
        label.Text = this.TrS(Localization.Menus.Menus_Inventory_DropMessage);
        _optionContainer = GetNode<OptionContainer>("%DropOptions");
        AddContainer(_optionContainer);
    }

    protected override void OnMockPreSetup()
    {
        BaseItem item = ItemsLocator.ItemDB.GetItem(ItemIds.Potion)!;
        _itemStack = new ItemStack(item, 3);
    }

    protected override void OnPreSetup(object? data)
    {
        if (data is not (int margin, ItemStack itemStack))
            return;
        _itemStack = itemStack;
        GetNode<MarginContainer>("%MarginContainer").SetLeftMargin(margin);
    }

    public override void HandleInput(IGUIInputHandler menuInput, double delta)
    {
        if (menuInput.Left.IsActionJustPressed)
            UpdateCount(-1);
        else if (menuInput.Right.IsActionJustPressed)
            UpdateCount(1);
        base.HandleInput(menuInput, delta);
    }

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem optionItem)
    {
        int total = _itemStack.Count;
        _inventory.RemoveItem(_itemStack, _count);
        if (_count == total)
            _ = CloseSubMenuAsync(cascadeTo: typeof(InventorySubMenu));
        else
            _ = CloseSubMenuAsync();
    }

    private void DisplayOptions()
    {
        _numberOption = _numberOptionScene.Instantiate<NumberOption>();
        if (_itemStack != null)
        {
            _numberOption.LabelText = $"{_count}/{_itemStack.Count}";
            _numberOption.Disabled = _itemStack.Count <= 0;
        }
        _optionContainer.AddOption(_numberOption);
    }

    private void UpdateCount(int num)
    {
        _count += num;
        if (_count < 1)
            _count = _itemStack.Count;
        else if (_count > _itemStack.Count)
            _count = 1;
        _numberOption.LabelText = $"{_count}/{_itemStack.Count}";
        Audio.PlaySoundFX(FocusedSoundPath);
    }
}
