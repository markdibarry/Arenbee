using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.ActionEffects;
using Arenbee.Actors;
using Arenbee.Game;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class UsePartySubMenu : OptionSubMenu
{
    public UsePartySubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _party = gameSession?.MainParty ?? new Party("temp");
        _inventory = gameSession?.MainParty?.Inventory ?? new Inventory();
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly AActionEffectDB _actionEffectDB = Locator.ActionEffectDB;
    private readonly Inventory _inventory;
    private readonly Party _party;
    private IActionEffect _actionEffect = null!;
    private ItemStack _itemStack = null!;
    private PackedScene _partyMemberOptionScene = GD.Load<PackedScene>(PartyMemberOption.GetScenePath());
    private OptionContainer _partyContainer = null!;
    private AItem Item => _itemStack.Item;

    public override void SetupData(object? data)
    {
        if (data is not ItemStack itemStack)
            return;
        _itemStack = itemStack;
        _actionEffect = _actionEffectDB.GetEffect(Item.UseData.ActionEffect)!;
    }

    protected override void SetupOptions()
    {
        DisplayOptions();
        base.SetupOptions();
    }

    protected override void OnItemSelected()
    {
        _ = HandleUse(CurrentContainer.FocusedItem);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _partyContainer = OptionContainers.Find(x => x.Name == "PartyOptions")!;
        if (_itemStack == null)
            return;
        if (_actionEffect.TargetType == (int)TargetType.PartyMemberAll)
        {
            _partyContainer.AllOptionEnabled = true;
            _partyContainer.SingleOptionsEnabled = false;
        }
    }

    private void DisplayOptions()
    {
        _partyContainer.Clear();
        if (_party == null)
            return;
        foreach (Actor? actor in _party.Actors.OrEmpty())
        {
            var option = _partyMemberOptionScene.Instantiate<PartyMemberOption>();
            _partyContainer.AddGridChild(option);

            option.OptionData[nameof(Actor)] = actor;
            option.NameLabel.Text = actor.Name;
            option.HPContainer.StatNameText = "HP";
            option.MPContainer.StatNameText = "MP";
        }
        UpdatePartyDisplay();
    }

    private void UpdatePartyDisplay()
    {
        if (_actionEffect.TargetType == (int)TargetType.PartyMemberAll)
            UpdatePartyAllDisplay();
        else
            UpdatePartySingleDisplay();
    }

    private void UpdatePartyAllDisplay()
    {
        bool canUse = _actionEffect.CanUse(null, _party.Actors.ToArray(), (int)ActionType.Item, Item.UseData.Value1, Item.UseData.Value2);

        foreach (PartyMemberOption option in _partyContainer.OptionItems.Cast<PartyMemberOption>())
        {
            if (!option.TryGetData(nameof(Actor), out Actor? actor))
                continue;
            option.Disabled = !canUse || _itemStack.Count <= 0;
            Stats stats = actor.Stats;
            option.HPContainer.StatCurrentValueText = stats.CurrentHP.ToString();
            option.HPContainer.StatMaxValueText = stats.MaxHP.ToString();
        }
    }

    private void UpdatePartySingleDisplay()
    {
        foreach (PartyMemberOption option in _partyContainer.OptionItems.Cast<PartyMemberOption>())
        {
            if (!option.TryGetData(nameof(Actor), out Actor? actor))
                continue;
            bool canUse = _actionEffect.CanUse(null, new[] { actor }, (int)ActionType.Item, Item.UseData.Value1, Item.UseData.Value2);
            option.Disabled = !canUse || _itemStack.Count <= 0;
            Stats stats = actor.Stats;
            option.HPContainer.StatCurrentValueText = stats.CurrentHP.ToString();
            option.HPContainer.StatMaxValueText = stats.MaxHP.ToString();
            option.MPContainer.StatCurrentValueText = "0";
            option.MPContainer.StatMaxValueText = "0";
        }
    }

    private async Task HandleUse(OptionItem? optionItem)
    {
        if ((optionItem == null || optionItem.Disabled) && !_partyContainer.AllOptionEnabled)
            return;
        IEnumerable<OptionItem> selectedItems;
        if (_partyContainer.AllOptionEnabled)
            selectedItems = _partyContainer.GetSelectedItems();
        else
            selectedItems = new OptionItem[] { optionItem };
        List<AActor> targets = new();
        foreach (OptionItem item in selectedItems)
        {
            if (!item.TryGetData(nameof(Actor), out Actor? actor))
                return;
            targets.Add(actor);
        }
        await CloseMenuAsync();
        _inventory.RemoveItem(_itemStack);
        _actionEffect.Use(null, targets, (int)ActionType.Item, Item.UseData.Value1, Item.UseData.Value2);
        //UpdatePartyDisplay();
    }
}
