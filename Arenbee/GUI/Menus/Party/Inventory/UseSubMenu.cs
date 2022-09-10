using System.Collections.Generic;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using Godot;

namespace Arenbee.GUI.Menus.Party;

[Tool]
public partial class UseSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private PackedScene _textOptionScene;
    private OptionContainer _optionContainer;
    public ItemStack ItemStack { get; set; }
    public ItemBase Item { get; set; }

    protected override void ReplaceDefaultOptions()
    {
        DisplayOptions();
        base.ReplaceDefaultOptions();
    }

    protected override void OnItemSelected()
    {
        base.OnItemSelected();
        var optionValue = CurrentContainer.CurrentItem.GetData<string>("value");
        if (optionValue == null)
            return;
        if (!CurrentContainer.CurrentItem.Disabled)
        {
            if (optionValue == "use")
                HandleUse();
            else if (optionValue == "drop")
                HandleDrop();
        }
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _optionContainer = OptionContainers.Find(x => x.Name == "UseOptions");
        _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        if (ItemStack == null)
            return;
        Item = ItemStack.Item;
    }

    private void DisplayOptions()
    {
        var options = new List<TextOption>();
        var option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = "Use";
        option.OptionData["value"] = "use";
        option.Disabled = true;
        if (Item.UseData != null)
        {
            option.Disabled = false;
            if (ItemStack.Amount <= 0)
                option.Disabled = true;
        }
        options.Add(option);

        option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = "Drop";
        option.OptionData["value"] = "drop";
        option.Disabled = !Item.IsDroppable || ItemStack.Amount <= 0;
        options.Add(option);
        _optionContainer.ReplaceChildren(options);
    }

    private void HandleDrop()
    {
        // TODO
    }

    private void HandleUse()
    {
        switch (Item.UseData.UseType)
        {
            case ItemUseType.Self:
            case ItemUseType.PartyMember:
            case ItemUseType.PartyMemberAll:
                OpenPartyUseSubMenu();
                break;
            case ItemUseType.Enemy:
            case ItemUseType.EnemyAll:
            case ItemUseType.EnemyCone:
            case ItemUseType.EnemyRadius:
                OpenEnemyUseSubMenu();
                break;
        }
    }

    private void OpenPartyUseSubMenu()
    {
        var usePartySubMenu = GDEx.Instantiate<UsePartySubMenu>(UsePartySubMenu.GetScenePath());
        usePartySubMenu.ItemStack = ItemStack;
        RaiseRequestedAdd(usePartySubMenu);
    }

    private void OpenEnemyUseSubMenu()
    {
        // TODO
    }
}
