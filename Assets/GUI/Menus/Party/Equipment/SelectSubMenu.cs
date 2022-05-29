using System.Collections.Generic;
using Arenbee.Assets.GUI.Menus.Common;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party.Equipment
{
    [Tool]
    public partial class SelectSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private string _currentItemId;
        private OptionContainer _equipOptions;
        private IItemDB _itemDB;
        private PackedScene _keyValueOptionScene;
        private Stats _mockStats;
        private PlayerParty _playerParty;
        private StatsDisplay _statsDisplay;
        public Actor Actor { get; set; }
        public EquipmentSlot Slot { get; set; }

        protected override void ReplaceDefaultOptions()
        {
            UpdateEquippableOptions();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);

            _itemDB.GetItem(_currentItemId)?.RemoveFromStats(_mockStats);
            _currentItemId = optionItem.GetData<string>("itemId");
            _itemDB.GetItem(_currentItemId)?.AddToStats(_mockStats);
            _statsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (TryEquip(optionItem.GetData<string>("itemId"), Slot))
                RaiseRequestedClose();
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _itemDB = Locator.GetItemDB();
            _mockStats = Actor?.Stats == null ? null : new Stats(Actor.Stats);
            _currentItemId = Slot?.ItemId;
            _equipOptions = OptionContainers.Find(x => x.Name == "EquipOptions");
            _statsDisplay = Foreground.GetNode<StatsDisplay>("StatsDisplay");
            _playerParty = Locator.GetParty() ?? new PlayerParty();
            _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
        }

        private bool TryEquip(string itemId, EquipmentSlot slot)
        {
            return Actor.Equipment.TrySetItemById(slot, itemId);
        }

        private void UpdateEquippableOptions()
        {
            var options = new List<KeyValueOption>();
            var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
            unequipOption.KeyText = "<Unequip>";
            unequipOption.ValueText = string.Empty;
            unequipOption.OptionData["itemId"] = null;
            options.Add(unequipOption);
            foreach (var itemStack in _playerParty.Inventory?.GetItemsByType(Slot.SlotType))
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Amount.ToString();
                option.OptionData["itemId"] = itemStack.ItemId;
                if (!itemStack.CanReserve())
                    option.Disabled = true;
                if (itemStack.ItemId == Slot.ItemId)
                    _equipOptions.ItemIndex = options.Count;
                options.Add(option);
            }
            _equipOptions.ReplaceChildren(options);
        }
    }
}
