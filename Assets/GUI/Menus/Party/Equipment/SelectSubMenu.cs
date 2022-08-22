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
        private ActorStatsDisplay _actorStatsDisplay;
        private ItemStatsDisplay _itemStatsDisplay;
        public Actor Actor { get; set; }
        public EquipmentSlot Slot { get; set; }

        protected override void ReplaceDefaultOptions()
        {
            UpdateEquippableOptions();
            _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
            _itemStatsDisplay.UpdateStatsDisplay(null);
        }

        protected override void OnItemFocused()
        {
            base.OnItemFocused();

            if (CurrentContainer.CurrentItem == null)
            {
                _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
                _itemStatsDisplay.UpdateStatsDisplay(null);
                return;
            }
            _itemDB.GetItem(_currentItemId)?.RemoveFromStats(_mockStats);
            _currentItemId = CurrentContainer.CurrentItem.GetData<string>("itemId");
            var newItem = _itemDB.GetItem(_currentItemId);
            newItem?.AddToStats(_mockStats);
            _actorStatsDisplay.UpdateStatsDisplay(Actor?.Stats, _mockStats);
            _itemStatsDisplay.UpdateStatsDisplay(newItem);
        }

        protected override void OnItemSelected()
        {
            base.OnItemSelected();
            if (TryEquip(CurrentContainer.CurrentItem.GetData<string>("itemId"), Slot))
                RaiseRequestedClose();
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _itemDB = Locator.GetItemDB();
            _mockStats = Actor?.Stats == null ? null : new Stats(Actor.Stats);
            _currentItemId = Slot?.ItemId;
            _equipOptions = OptionContainers.Find(x => x.Name == "EquipOptions");
            _actorStatsDisplay = Foreground.GetNode<ActorStatsDisplay>("ActorStatsDisplay");
            _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
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
            //unequipOption.OptionData["itemId"] = null;
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
                    _equipOptions.CurrentIndex = options.Count;
                options.Add(option);
            }
            _equipOptions.ReplaceChildren(options);
        }
    }
}
