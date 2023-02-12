using System.Collections.Generic;
using System.Linq;
using Arenbee.Game;
using Arenbee.GUI.Menus.Common;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Party;

[Tool]
public partial class UsePartySubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private PackedScene _partyMemberOptionScene;
    private OptionContainer _partyContainer;
    public AItemStack ItemStack { get; set; }
    public AItem Item { get; set; }
    public PlayerParty Party { get; set; }
    public ActionEffectDBBase ActionEffectDB { get; set; }
    public IActionEffect ActionEffect { get; set; }

    public override void SetupData(object data)
    {
        if (data is not AItemStack itemStack)
            return;
        ItemStack = itemStack;
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
        Party = Locator.Session?.Party;
        ActionEffectDB = Locator.ActionEffectDB;
        _partyContainer = OptionContainers.Find(x => x.Name == "PartyOptions");
        _partyMemberOptionScene = GD.Load<PackedScene>(PartyMemberOption.GetScenePath());
        if (ItemStack == null)
            return;
        Item = ItemStack.Item;
        if (Item.UseData.UseType == ItemUseType.PartyMemberAll)
        {
            _partyContainer.AllOptionEnabled = true;
            _partyContainer.SingleOptionsEnabled = false;
        }
        ActionEffect = ActionEffectDB.GetEffect(Item.UseData.ActionEffect);
    }

    private void DisplayOptions()
    {
        _partyContainer.Clear();
        if (Party == null)
            return;
        foreach (var actor in Party.Actors.OrEmpty())
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
            ActionType = ActionType.Item,
            Value1 = Item.UseData.Value1,
            Value2 = Item.UseData.Value2
        };
        foreach (PartyMemberOption option in _partyContainer.OptionItems.Cast<PartyMemberOption>())
        {
            if (!option.TryGetData("actor", out AActor? actor))
                continue;
            var target = new AActor[] { actor };
            bool canUse = ActionEffect.CanUse(request, target);
            option.Disabled = !canUse || ItemStack.Count <= 0;
            option.HPContainer.StatCurrentValueText = actor.Stats.GetHP().ToString();
            option.HPContainer.StatMaxValueText = actor.Stats.GetMaxHP().ToString();
            option.MPContainer.StatCurrentValueText = actor.Stats.GetMP().ToString();
            option.MPContainer.StatMaxValueText = actor.Stats.GetMaxMP().ToString();
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
            ActionType = ActionType.Item,
            Value1 = Item.UseData.Value1,
            Value2 = Item.UseData.Value2
        };
        foreach (OptionItem item in selectedItems)
        {
            if (!item.TryGetData("actor", out AActorBody? actor))
                return;
            ActionEffect.Use(request, new AActorBody[] { actor });
        }
        _inventory.RemoveItem(ItemStack);

        UpdatePartyDisplay();
    }
}
