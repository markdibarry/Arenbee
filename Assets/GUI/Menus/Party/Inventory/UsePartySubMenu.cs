using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class UsePartySubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private PackedScene _partyMemberOptionScene;
        private OptionContainer _partyContainer;
        public ItemStack ItemStack { get; set; }
        public Item Item { get; set; }
        public PlayerParty Party { get; set; }

        protected override void ReplaceDefaultOptions()
        {
            DisplayOptions();
            base.ReplaceDefaultOptions();
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            HandleUse(optionItem);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            Party = Locator.GetParty();
            _partyContainer = OptionContainers.Find(x => x.Name == "Party");
            _partyMemberOptionScene = GD.Load<PackedScene>(PartyMemberOption.GetScenePath());
            Item = ItemStack.Item;
            if (Item.UseData.UseType == ItemUseType.PartyMemberAll)
            {
                _partyContainer.AllOptionEnabled = true;
                _partyContainer.SingleOptionsEnabled = false;
            }
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
            foreach (PartyMemberOption option in _partyContainer.OptionItems.Cast<PartyMemberOption>())
            {
                var actor = option.GetData<Actor>("actor");
                if (actor == null)
                    continue;
                option.Disabled = !Item.UseData.CanUse(actor) || ItemStack.Amount <= 0;
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

            foreach (OptionItem item in selectedItems)
            {
                var actor = optionItem.GetData<Actor>("actor");
                if (actor == null)
                    return;
                    
            }
        }
    }
}
