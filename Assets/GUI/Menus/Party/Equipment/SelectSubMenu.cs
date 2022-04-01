using System.Collections.Generic;
using Arenbee.Assets.GUI.Menus.Common;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party.Equipment
{
    [Tool]
    public partial class SelectSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private IPlayerParty _playerParty;
        private OptionContainer _equipOptions;
        private StatsDisplay _statsDisplay;
        private PackedScene _keyValueOptionScene;
        public Actor Actor { get; set; }
        public EquipmentSlot Slot { get; set; }

        protected override void CustomOptionsSetup()
        {
            _playerParty = Locator.GetParty();
            _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
            AddItemOptions();
            base.CustomOptionsSetup();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            _statsDisplay.Update(Actor, Slot, optionItem.GetData("itemId"));
        }

        protected override async void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (TryEquip(optionItem.GetData("itemId"), Slot))
                await CloseSubMenuAsync();
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _equipOptions = Foreground.GetNode<OptionContainer>("EquipOptions");
            OptionContainers.Add(_equipOptions);
            _statsDisplay = Foreground.GetNode<StatsDisplay>("StatsDisplay");
        }

        private void AddItemOptions()
        {
            var options = new List<KeyValueOption>();
            var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
            unequipOption.KeyText = "<Unequip>";
            unequipOption.ValueText = string.Empty;
            unequipOption.OptionData.Add("itemId", null);
            options.Add(unequipOption);
            foreach (var itemStack in _playerParty.Inventory.GetItemsByType(Slot.SlotType))
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Amount.ToString();
                option.OptionData.Add("itemId", itemStack.ItemId);
                if (!itemStack.CanReserve())
                    option.Disabled = true;
                if (itemStack.ItemId == Slot.ItemId)
                    _equipOptions.ItemIndex = options.Count;
                options.Add(option);
            }
            _equipOptions.ReplaceChildren(options);
            _equipOptions.InitItems();
        }

        private bool TryEquip(string itemId, EquipmentSlot slot)
        {
            return Actor.Equipment.TrySetItemById(slot, itemId);
        }
    }
}
