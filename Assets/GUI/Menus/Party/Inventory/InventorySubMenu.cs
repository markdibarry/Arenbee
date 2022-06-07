using System;
using System.Collections.Generic;
using Arenbee.Assets.GUI.Menus.Common;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class InventorySubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private Inventory _inventory;
        private OptionContainer _inventoryList;
        private DynamicTextContainer _itemInfo;
        private ItemStatsDisplay _itemStatsDisplay;
        private OptionContainer _typeList;
        private PackedScene _keyValueOptionScene;

        public override void HandleInput(float delta)
        {
            if (MenuInput.Cancel.IsActionJustPressed && CurrentContainer == _inventoryList)
            {
                UpdateItemDescription(null);
                FocusContainer(_typeList);
            }
            else
            {
                base.HandleInput(delta);
            }
        }

        protected override void ReplaceDefaultOptions()
        {
            var typeOptions = GetItemTypeOptions();
            _typeList.GridContainer.Columns = typeOptions.Count;
            _typeList.ReplaceChildren(typeOptions);

            var itemOptions = GetItemOptions(ItemType.None);
            _inventoryList.ReplaceChildren(itemOptions);

            UpdateItemDescription(null);
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            _itemInfo.UpdateText(string.Empty);
            if (optionContainer == _typeList)
                UpdateItemList(optionItem);
            else
                UpdateItemDescription(optionItem);
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (optionContainer == _typeList)
                FocusContainer(_inventoryList);
            else if (optionContainer == _inventoryList)
                OpenUseSubMenu(optionItem);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _typeList = OptionContainers.Find(x => x.Name == "TypeList");
            _inventoryList = OptionContainers.Find(x => x.Name == "InventoryList");
            _itemInfo = Foreground.GetNode<DynamicTextContainer>("ItemInfo");
            _itemStatsDisplay = Foreground.GetNode<ItemStatsDisplay>("ItemStatsDisplay");
            _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
            _inventory = Locator.GetParty()?.Inventory;
        }

        private List<TextOption> GetItemTypeOptions()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            var allOption = textOptionScene.Instantiate<TextOption>();
            allOption.LabelText = "All";
            allOption.OptionData["typeName"] = "None";
            var options = new List<TextOption>() { allOption };
            foreach (var itemType in Enum<ItemType>.Values())
            {
                if (itemType == ItemType.None) continue;
                var option = textOptionScene.Instantiate<TextOption>();
                option.LabelText = itemType.Get().Name;
                option.OptionData["typeName"] = itemType.Get().Name;
                options.Add(option);
            }
            return options;
        }

        private List<KeyValueOption> GetItemOptions(ItemType itemType = ItemType.None)
        {
            var options = new List<KeyValueOption>();
            ICollection<ItemStack> itemStacks;
            if (itemType == ItemType.None)
                itemStacks = _inventory?.Items;
            else
                itemStacks = _inventory?.GetItemsByType(itemType);

            foreach (var itemStack in itemStacks)
            {
                var option = _keyValueOptionScene.Instantiate<KeyValueOption>();
                option.KeyText = itemStack.Item.DisplayName;
                option.ValueText = "x" + itemStack.Amount.ToString();
                option.OptionData["itemStack"] = itemStack;
                options.Add(option);
            }
            return options;
        }

        private void OpenUseSubMenu(OptionItem optionItem)
        {
            var itemStack = optionItem.GetData<ItemStack>("itemStack");
            if (itemStack == null)
                return;
            var useSubMenu = GDEx.Instantiate<UseSubMenu>(UseSubMenu.GetScenePath());
            useSubMenu.ItemStack = itemStack;
            RaiseRequestedAdd(useSubMenu);
        }

        private void UpdateItemDescription(OptionItem optionItem)
        {
            Item item = optionItem?.GetData<ItemStack>("itemStack")?.Item;
            _itemStatsDisplay.UpdateStatsDisplay(item);
            _itemInfo.UpdateText(item?.Description);
        }

        private void UpdateItemList(OptionItem optionItem)
        {
            if (optionItem == null)
                return;
            _inventoryList.ResetContainerFocus();
            ItemType itemType = ItemType.None;
            string typeName = optionItem.GetData<string>("typeName");
            if (typeName == null)
                return;
            if (typeName != "All")
                itemType = Enum.Parse<ItemType>(typeName);
            List<KeyValueOption> options = GetItemOptions(itemType);
            _inventoryList.ReplaceChildren(options);
        }
    }
}
