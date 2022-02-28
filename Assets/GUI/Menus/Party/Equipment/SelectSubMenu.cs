using System.Collections.Generic;
using System.Threading.Tasks;
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
        private IItemDB _itemDB;
        private IPlayerParty _playerParty;
        private OptionContainer _equipOptions;
        private GridContainer _statsDisplayGrid;
        private PackedScene _keyValueOptionScene;
        public Actor Actor { get; set; }
        public EquipmentSlot Slot { get; set; }

        protected override void CustomOptionsSetup()
        {
            _itemDB = Locator.GetItemDB();
            _playerParty = Locator.GetParty();
            _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
            AddItemOptions();
            base.CustomOptionsSetup();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            UpdateStatsDisplay(optionItem);
        }

        protected override async void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (TryEquip(optionItem, Slot))
                await CloseSubMenuAsync();
        }

        private bool TryEquip(OptionItem optionItem, EquipmentSlot slot)
        {
            if (!optionItem.OptionData.TryGetValue("itemId", out string itemId))
                return false;
            if (!optionItem.OptionData.TryGetValue("canSelect", out string canSelect))
                return false;
            if (itemId == "Unequip")
            {
                slot.SetItem(null);
                return true;
            }
            else if (bool.Parse(canSelect))
            {
                slot.SetItem(_itemDB.GetItem(itemId));
                return true;
            }
            return false;
        }

        private void UpdateStatsDisplay(OptionItem optionItem)
        {
            Framework.Items.Equipment mockEquipment = Actor.Equipment.CloneEquipment();
            bool equipSuccess = TryEquip(optionItem, mockEquipment.GetSlot(Slot.SlotName));
            Stats mockStats = mockEquipment.GenerateStats(Actor.Stats);
            foreach (var attributePair in Actor.Stats.Attributes)
            {
                var statContainer = _statsDisplayGrid.GetNodeOrNull<MarginContainer>(attributePair.Key.ToString());
                if (statContainer == null) continue;
                var currentValue = attributePair.Value.DisplayValue;
                var mockNewValue = mockStats.Attributes[attributePair.Key].DisplayValue;
                var valueLabel = statContainer.GetNode<Label>("HBoxContainer/Values/Value");
                var newValueLabel = statContainer.GetNode<Label>("HBoxContainer/Values/NewValue");
                if (equipSuccess && currentValue != mockNewValue)
                {
                    valueLabel.Text = currentValue.ToString() + " ->";
                    newValueLabel.Text = mockNewValue.ToString();
                    if (mockNewValue > currentValue)
                        newValueLabel.Modulate = new Color(0.7f, 1f, 0.7f, 1f);
                    else if (mockNewValue < currentValue)
                        newValueLabel.Modulate = new Color(1f, 0.7f, 0.7f, 1f);
                    else
                        newValueLabel.Modulate = Colors.White;
                    newValueLabel.Show();
                }
                else
                {
                    valueLabel.Text = currentValue.ToString();
                    newValueLabel.Hide();
                }
            }
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _equipOptions = Foreground.GetNode<OptionContainer>("EquipOptions");
            OptionContainers.Add(_equipOptions);
            _statsDisplayGrid = Foreground.GetNode<GridContainer>("StatsDisplay/GridContainer");
        }

        private void AddItemOptions()
        {
            var options = new List<KeyValueOption>();
            var unequipOption = _keyValueOptionScene.Instantiate<KeyValueOption>();
            unequipOption.KeyText = "<Unequip>";
            unequipOption.ValueText = string.Empty;
            unequipOption.OptionData.Add("itemId", "Unequip");
            unequipOption.OptionData.Add("canSelect", true.ToString());
            options.Add(unequipOption);
            ICollection<ItemStack> itemStacks = _playerParty.Inventory.GetItemsByType(Slot.SlotType);
            foreach (var itemStack in itemStacks)
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Amount.ToString();
                option.OptionData.Add("itemId", itemStack.ItemId);
                option.OptionData.Add("canSelect", itemStack.CanReserve().ToString());
                if (!itemStack.CanReserve())
                    option.CanHighlight = false;
                options.Add(option);
            }
            _equipOptions.ReplaceChildren(options);
        }
    }
}
