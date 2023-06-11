using System.Collections.Generic;
using Arenbee.ActionEffects;
using GameCore.ActionEffects;
using GameCore;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class UseSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private ItemStack _itemStack = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    private OptionContainer _optionContainer = null!;
    private Control _referenceContainer = null!;
    private int _referenceMargin;
    private readonly ActionEffectDB _actionEffectDB = (ActionEffectDB)Locator.ActionEffectDB;

    protected override void OnSetup()
    {
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        _referenceContainer = GetNode<Control>("%VBoxContainer");
        _referenceContainer.ItemRectChanged += OnRefRectChanged;
        _optionContainer = GetNode<OptionContainer>("%UseOptions");
        AddContainer(_optionContainer);

        DisplayOptions();
    }

    protected override void OnPreSetup(object? data)
    {
        if (data is not (int margin, ItemStack itemStack))
            return;
        _itemStack = itemStack;
        GetNode<MarginContainer>("%MarginContainer").SetLeftMargin(margin);
    }

    protected void OnRefRectChanged()
    {
        _referenceMargin = (int)(_referenceContainer.Position.X + _referenceContainer.Size.X);
    }

    protected override void OnItemPressed(OptionContainer optionContainer, OptionItem? optionItem)
    {
        if (optionItem == null || optionItem.Disabled || optionItem.OptionData is not string optionValue)
            return;
        if (optionValue == Localization.Menus.Menus_Inventory_Use_Use)
            HandleUse();
        else if (optionValue == Localization.Menus.Menus_Inventory_Use_Drop)
            HandleDrop();
    }

    private void DisplayOptions()
    {
        List<TextOption> options = new();
        TextOption option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = this.TrS(Localization.Menus.Menus_Inventory_Use_Use);
        option.OptionData = Localization.Menus.Menus_Inventory_Use_Use;
        option.Disabled = true;
        if (_itemStack?.Item.UseData != null)
            option.Disabled = _itemStack.Count <= 0;
        options.Add(option);

        option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = this.TrS(Localization.Menus.Menus_Inventory_Use_Drop);
        option.OptionData = Localization.Menus.Menus_Inventory_Use_Drop;
        if (_itemStack != null)
            option.Disabled = !_itemStack.Item.IsDroppable || _itemStack.Count <= 0;
        options.Add(option);
        _optionContainer.ReplaceChildren(options);
    }

    private void HandleDrop()
    {
        _ = OpenSubMenuAsync(path: DropSubMenu.GetScenePath(), data: (_referenceMargin, _itemStack));
    }

    private void HandleUse()
    {
        IActionEffect? effect = _actionEffectDB.GetEffect(_itemStack.Item.UseData.ActionEffect);
        if (effect == null)
            return;
        switch ((TargetType)effect.TargetType)
        {
            case TargetType.Self:
            case TargetType.PartyMember:
            case TargetType.PartyMemberAll:
                OpenPartyUseSubMenu();
                break;
            case TargetType.Enemy:
            case TargetType.EnemyAll:
            case TargetType.EnemyCone:
            case TargetType.EnemyRadius:
                OpenEnemyUseSubMenu();
                break;
        }
    }

    private void OpenPartyUseSubMenu()
    {
        _ = OpenSubMenuAsync(path: UsePartySubMenu.GetScenePath(), data: (_referenceMargin, _itemStack));
    }

    private void OpenEnemyUseSubMenu()
    {
        _ = OpenSubMenuAsync(path: UseEnemySubMenu.GetScenePath(), data: _itemStack);
    }
}
