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
        private Label ItemInfoLabel { get; set; }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            ItemInfoLabel = GetNode<Label>("ItemInfo/Control/MarginContainer/ItemInfoLabel");
        }

        protected override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
        }

        protected override void AddContainerItems()
        {
            AddInventoryTypes();
            AddInventoryItems();
        }

        protected override void OnItemFocused(OptionItem optionItem)
        {
            base.OnItemFocused(optionItem);
            if (optionItem is KeyValueOption)
            {
                Item item = ItemDB.GetItem(optionItem.OptionValue);
                if (item != null)
                    ItemInfoLabel.Text = item.Description;
            }
        }

        protected override void OnFocusOOB(OptionContainer container, Direction direction)
        {
            if (container.Name == "TypeList")
            {
                HandleTypeListOOB(container, direction);
            }
            else
            {
                HandleInventoryListOOB(container, direction);
            }
        }

        private void HandleTypeListOOB(OptionContainer container, Direction direction)
        {
            if (direction == Direction.Down)
            {
                FocusContainer(OptionContainers.First(x => x.Name == "InventoryList"));
            }
            else
            {
                base.OnFocusOOB(container, direction);
            }
        }

        private void HandleInventoryListOOB(OptionContainer container, Direction direction)
        {
            if (direction == Direction.Up)
            {
                FocusContainer(OptionContainers.First(x => x.Name == "TypeList"));
            }
            else
            {
                base.OnFocusOOB(container, direction);
            }
        }

        private void AddInventoryItems()
        {
            var keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.ScenePath);
            var options = new List<KeyValueOption>();
            Inventory inventory = GameRoot.Instance.CurrentGame.Party.Inventory;
            foreach (var item in inventory.Items)
            {
                var option = keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = ItemDB.GetItem(item.ItemId).DisplayName;
                option.ValueText = "x" + item.Amount.ToString();
                option.OptionValue = item.ItemId;
                options.Add(option);
            }
            OptionContainer inventoryList = OptionContainers.Find(x => x.Name == "InventoryList");

            inventoryList.ReplaceItems(options);
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
