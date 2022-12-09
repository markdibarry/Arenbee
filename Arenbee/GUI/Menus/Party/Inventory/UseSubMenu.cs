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

    protected override void SetupOptions()
    {
        _optionContainer = OptionContainers.Find(x => x.Name == "UseOptions");
        DisplayOptions();
    }

    public override void SetupData(object data)
    {
        if (data is not ItemStack itemStack)
            return;
        ItemStack = itemStack;
    }

    protected override void OnItemSelected()
    {
        var optionValue = CurrentContainer.CurrentItem.GetData<string>("value");
        if (optionValue == null)
            return;
        if (!CurrentContainer.CurrentItem.Disabled)
        {
            if (optionValue == UseOptions.Use)
                HandleUse();
            else if (optionValue == UseOptions.Drop)
                HandleDrop();
        }
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    }

    private void DisplayOptions()
    {
        var options = new List<TextOption>();
        var option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = UseOptions.Use;
        option.OptionData["value"] = UseOptions.Use;
        option.Disabled = true;
        if (ItemStack.Item.UseData != null)
        {
            option.Disabled = false;
            if (ItemStack.Amount <= 0)
                option.Disabled = true;
        }
        options.Add(option);

        option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = UseOptions.Drop;
        option.OptionData["value"] = UseOptions.Drop;
        option.Disabled = !ItemStack.Item.IsDroppable || ItemStack.Amount <= 0;
        options.Add(option);
        _optionContainer.ReplaceChildren(options);
    }

    private static void HandleDrop()
    {
        // TODO
    }

    private void HandleUse()
    {
        switch (ItemStack.Item.UseData.UseType)
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
        _ = OpenSubMenuAsync(path: UsePartySubMenu.GetScenePath(), data: ItemStack);
    }

    private static void OpenEnemyUseSubMenu()
    {
        // TODO
    }

    private static class UseOptions
    {
        public static List<string> GetAll()
        {
            return new List<string>()
            {
                Use,
                Drop
            };
        }
        public const string Use = "Use";
        public const string Drop = "Drop";
    }
}
