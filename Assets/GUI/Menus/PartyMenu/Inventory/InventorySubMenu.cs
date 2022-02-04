using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class InventorySubMenu : OptionSubMenu
    {
        public static new readonly string ScenePath = $"res://Assets/GUI/Menus/PartyMenu/Inventory/{nameof(InventorySubMenu)}.tscn";
        [Export]
        private readonly NodePath _itemInfoLabelPath;
        private Label _itemInfoLabel;
        private OptionContainer _inventoryList;
        private OptionContainer _typeList;
        private Inventory _inventory;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _inventoryList = OptionContainers[0];
            _typeList = OptionContainers[1];
            _itemInfoLabel = GetNode<Label>(_itemInfoLabelPath);
        }

        protected override Task Init()
        {
            _inventory = GameRoot.Instance.CurrentGame.Party.Inventory;
            return base.Init();
        }

        protected override void AddContainerItems()
        {
            _inventoryList?.ReplaceItems(GetItemOptions(null));

            if (_typeList != null)
            {
                var typeOptions = GetItemTypeOptions();
                _typeList.GridContainer.Columns = typeOptions.Count;
                _typeList.ReplaceItems(typeOptions);
            }
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (optionContainer == _typeList)
            {
                ItemType? itemType = null;
                if (optionItem.OptionValue != "All")
                    itemType = Enum.Parse<ItemType>(optionItem.OptionValue);
                var options = GetItemOptions(itemType);
                _inventoryList.ReplaceItems(options);
                _inventoryList.InitItems();
            }
            else
            {
                Item item = _inventory.GetItemStack(optionItem.OptionValue)?.Item;
                if (item != null)
                    _itemInfoLabel.Text = item.Description;
            }
        }

        protected override void OnFocusOOB(OptionContainer containerLeavingFocus, Direction direction)
        {
            if (containerLeavingFocus == _typeList)
                HandleTypeListOOB(containerLeavingFocus, direction);
            else
                HandleInventoryListOOB(containerLeavingFocus, direction);
        }

        private void HandleTypeListOOB(OptionContainer container, Direction direction)
        {
            if (direction == Direction.Down)
                FocusContainerPreviousItem(_inventoryList);
            else
                base.OnFocusOOB(container, direction);
        }

        private void HandleInventoryListOOB(OptionContainer container, Direction direction)
        {
            if (direction == Direction.Up)
            {
                _itemInfoLabel.Text = string.Empty;
                FocusContainerPreviousItem(_typeList);
            }
            else
            {
                base.OnFocusOOB(container, direction);
            }
        }

        private List<KeyValueOption> GetItemOptions(ItemType? itemType)
        {
            var keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.ScenePath);
            var options = new List<KeyValueOption>();
            Inventory inventory = GameRoot.Instance.CurrentGame.Party.Inventory;
            ICollection<ItemStack> itemStacks;
            if (itemType == null)
                itemStacks = inventory.Items;
            else
                itemStacks = inventory.GetItemsByType((ItemType)itemType);

            foreach (var itemStack in itemStacks)
            {
                var option = keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Amount.ToString();
                option.OptionValue = itemStack.ItemId;
                options.Add(option);
            }
            return options;
        }

        private List<TextOption> GetItemTypeOptions()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.ScenePath);
            var allOption = textOptionScene.Instantiate<TextOption>();
            allOption.LabelText = "All";
            allOption.OptionValue = "All";
            var options = new List<TextOption>() { allOption };
            foreach (var typeName in Enum.GetNames(typeof(ItemType)))
            {
                var option = textOptionScene.Instantiate<TextOption>();
                option.LabelText = typeName;
                option.OptionValue = typeName;
                options.Add(option);
            }
            return options;
        }
    }
}
