using System.Collections.Generic;
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

        private void AddInventoryItems()
        {
            var inventoryOptionScene = GD.Load<PackedScene>(KeyValueOption.ScenePath);
            var options = new List<KeyValueOption>();
            Inventory inventory = GameRoot.Instance.CurrentGame.Party.Inventory;
            foreach (var item in inventory.Items)
            {
                var option = inventoryOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = ItemDB.GetItem(item.ItemId).DisplayName;
                option.ValueText = "x" + item.Amount.ToString();
                option.OptionValue = item.ItemId;
                options.Add(option);
            }
            OptionContainer inventoryList = OptionContainers.Find(x => x.Name == "InventoryList");

            inventoryList.ReplaceItems(options);
        }
    }
}
