using System;
using System.Collections.Generic;
using System.Linq;
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
        private string _inventoryListName;
        private string _typeListName;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _inventoryListName = OptionContainers[0].Name;
            _typeListName = OptionContainers[1].Name;
            _itemInfoLabel = GetNode<Label>(_itemInfoLabelPath);
        }

        protected override void AddContainerItems()
        {
            AddInventoryTypes();
            ReplaceInventoryItems(null);
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (optionContainer.Name == _inventoryListName)
                HandleInventoryItemSelected(optionItem);
            else
                HandleTypeItemSelected(optionItem);
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (optionContainer.Name == _typeListName)
            {
                ItemType? itemType = null;
                if (optionItem.OptionValue != "All")
                    itemType = Enum.Parse<ItemType>(optionItem.OptionValue);
                ReplaceInventoryItems(itemType);
            }
            else
            {
                Item item = ItemDB.GetItem(optionItem.OptionValue);
                if (item != null)
                    _itemInfoLabel.Text = item.Description;
            }
        }

        protected override void OnFocusOOB(OptionContainer container, Direction direction)
        {
            if (container.Name == _typeListName)
                HandleTypeListOOB(container, direction);
            else
                HandleInventoryListOOB(container, direction);
        }

        private void HandleInventoryItemSelected(OptionItem optionItem)
        {

        }

        private void HandleTypeItemSelected(OptionItem optionItem)
        {
        }

        private void HandleTypeListOOB(OptionContainer container, Direction direction)
        {
            if (direction == Direction.Down)
                FocusContainer(OptionContainers.First(x => x.Name == _inventoryListName));
            else
                base.OnFocusOOB(container, direction);
        }

        private void HandleInventoryListOOB(OptionContainer container, Direction direction)
        {
            if (direction == Direction.Up)
                FocusContainer(OptionContainers.First(x => x.Name == _typeListName));
            else
                base.OnFocusOOB(container, direction);
        }

        private void ReplaceInventoryItems(ItemType? itemType)
        {
            var keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.ScenePath);
            var options = new List<KeyValueOption>();
            Inventory inventory = GameRoot.Instance.CurrentGame.Party.Inventory;
            List<ItemStack> items;
            if (itemType == null)
                items = inventory.Items.ToList();
            else
                items = inventory.GetItemsByType((ItemType)itemType).ToList();

            foreach (var item in items)
            {
                var option = keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = ItemDB.GetItem(item.ItemId).DisplayName;
                option.ValueText = "x" + item.Amount.ToString();
                option.OptionValue = item.ItemId;
                options.Add(option);
            }
            OptionContainer inventoryList = OptionContainers.Find(x => x.Name == "InventoryList");

            inventoryList.ReplaceItems(options);
            inventoryList.InitItems();
        }

        private void AddInventoryTypes()
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

            OptionContainer typeList = OptionContainers.Find(x => x.Name == "TypeList");
            typeList.GridContainer.Columns = options.Count;
            typeList.ReplaceItems(options);
        }
    }
}
