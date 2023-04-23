using System.Collections.Generic;
using Arenbee.ActionEffects;
using Arenbee.Items;
using GameCore.ActionEffects;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class UseSubMenu : OptionSubMenu
{
    public UseSubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private ItemStack _itemStack = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    private OptionContainer _optionContainer = null!;
    private readonly ActionEffectDB _actionEffectDB = (ActionEffectDB)Locator.ActionEffectDB;

    protected override void SetupOptions()
    {
        _optionContainer = OptionContainers.Find(x => x.Name == "UseOptions");
        DisplayOptions();
    }

    public override void SetupData(object? data)
    {
        if (data is not ItemStack itemStack)
            return;
        _itemStack = itemStack;
    }

    protected override void OnItemSelected()
    {
        if (CurrentContainer?.FocusedItem?.OptionData is not string optionValue)
            return;
        if (CurrentContainer.FocusedItem.Disabled)
            return;
        if (optionValue == UseOptions.Use)
            HandleUse();
        else if (optionValue == UseOptions.Drop)
            HandleDrop();
    }

    private void DisplayOptions()
    {
        List<TextOption> options = new();
        TextOption option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = UseOptions.Use;
        option.OptionData = UseOptions.Use;
        option.Disabled = true;
        if (_itemStack.Item.UseData != null)
            option.Disabled = _itemStack.Count <= 0;
        options.Add(option);

        option = _textOptionScene.Instantiate<TextOption>();
        option.LabelText = UseOptions.Drop;
        option.OptionData = UseOptions.Drop;
        option.Disabled = !_itemStack.Item.IsDroppable || _itemStack.Count <= 0;
        options.Add(option);
        _optionContainer.ReplaceChildren(options);
    }

    private void HandleDrop()
    {
        _ = OpenSubMenuAsync(path: DropSubMenu.GetScenePath(), data: _itemStack);
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
        _ = OpenSubMenuAsync(path: UsePartySubMenu.GetScenePath(), data: _itemStack);
    }

    private void OpenEnemyUseSubMenu()
    {
        _ = OpenSubMenuAsync(path: UseEnemySubMenu.GetScenePath(), data: _itemStack);
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
