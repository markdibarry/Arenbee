using System.Collections.Generic;
using System.Linq;
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
        _actionEffect = _actionEffectDB.GetEffect(_item.UseData.ActionEffect)!;
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly ActionEffectDBBase _actionEffectDB = Locator.ActionEffectDB;
    private readonly Inventory _inventory;
    private readonly Party _party;
    private IActionEffect _actionEffect;
    private ItemStack _itemStack = null!;
    private AItem _item = null!;
    private PackedScene _partyMemberOptionScene = GD.Load<PackedScene>(PartyMemberOption.GetScenePath());
    private OptionContainer _partyContainer = null!;

    public override void SetupData(object? data)
    {
        if (data is not ItemStack itemStack)
            return;
        _itemStack = itemStack;
        _item = _itemStack.Item;
    }

    protected override void SetupOptions()
    {
        DisplayOptions();
        base.SetupOptions();
    }

    protected override void OnItemSelected()
    {
        HandleUse(CurrentContainer.FocusedItem);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _partyContainer = OptionContainers.Find(x => x.Name == "PartyOptions")!;
        if (_itemStack == null)
            return;
        if (_item.UseData.UseType == ItemUseType.PartyMemberAll)
        {
            _partyContainer.AllOptionEnabled = true;
            _partyContainer.SingleOptionsEnabled = false;
        }
        _actionEffect = _actionEffectDB.GetEffect(_item.UseData.ActionEffect)!;
    }

    private void DisplayOptions()
    {
        _partyContainer.Clear();
        if (_party == null)
            return;
        foreach (var actor in _party.Actors.OrEmpty())
        {
            var option = _partyMemberOptionScene.Instantiate<PartyMemberOption>();
            _partyContainer.AddGridChild(option);

            option.OptionData["actor"] = actor;
            option.NameLabel.Text = actor.Name;
            option.HPContainer.StatNameText = "HP";
            option.MPContainer.StatNameText = "MP";
        }
        UpdatePartyDisplay();
    }

    private void UpdatePartyDisplay()
    {
        var request = new ActionEffectRequest()
        {
            ActionType = (int)ActionType.Item,
            Value1 = _item.UseData.Value1,
            Value2 = _item.UseData.Value2
        };
        foreach (PartyMemberOption option in _partyContainer.OptionItems.Cast<PartyMemberOption>())
        {
            if (!option.TryGetData("actor", out Actor? actor))
                continue;
            var target = new AActor[] { actor };
            bool canUse = _actionEffect.CanUse(request, target);
            option.Disabled = !canUse || _itemStack.Count <= 0;
            Stats stats = actor.Stats;
            option.HPContainer.StatCurrentValueText = stats.CurrentHP.ToString();
            option.HPContainer.StatMaxValueText = stats.MaxHP.ToString();
        }
    }

    private void HandleUse(OptionItem optionItem)
    {
        if (optionItem?.Disabled == true)
            return;
        IEnumerable<OptionItem> selectedItems;
        if (optionItem == null && _partyContainer.AllOptionEnabled)
            selectedItems = _partyContainer.GetSelectedItems();
        else
            selectedItems = new OptionItem[] { optionItem };
        var request = new ActionEffectRequest()
        {
            ActionType = (int)ActionType.Item,
            Value1 = _item.UseData.Value1,
            Value2 = _item.UseData.Value2
        };
        foreach (OptionItem item in selectedItems)
        {
            if (!item.TryGetData("actor", out Actor? actor))
                return;
            _actionEffect.Use(request, new Actor[] { actor });
        }
        _inventory.RemoveItem(_itemStack);

        UpdatePartyDisplay();
    }
}
