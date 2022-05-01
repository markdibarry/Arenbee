using System;
using System.Collections.Generic;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Input;
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
        private OptionContainer _typeList;
        private PackedScene _keyValueOptionScene;

        public override void HandleInput(GUIInputHandler input, float delta)
        {
            if (input.Cancel.IsActionJustPressed && CurrentContainer == _inventoryList)
                FocusContainer(_typeList);
            else
                base.HandleInput(input, delta);
        }

        protected override void ReplaceDefaultOptions()
        {
            var typeOptions = GetItemTypeOptions();
            _typeList.GridContainer.Columns = typeOptions.Count;
            _typeList.ReplaceChildren(typeOptions);

            var itemOptions = GetItemOptions(ItemType.None);
            _inventoryList.ReplaceChildren(itemOptions);
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
            _keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
            _inventory = Locator.GetParty()?.Inventory;
        }

        private List<TextOption> GetItemTypeOptions()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            var allOption = textOptionScene.Instantiate<TextOption>();
            allOption.LabelText = "All";
            allOption.OptionData.Add("typeName", "None");
            var options = new List<TextOption>() { allOption };
            foreach (var itemType in Enum<ItemType>.Values())
            {
                if (itemType == ItemType.None) continue;
                var option = textOptionScene.Instantiate<TextOption>();
                option.LabelText = itemType.Get().Name;
                option.OptionData.Add("typeName", itemType.Get().Name);
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
                option.OptionData.Add("itemId", itemStack.ItemId);
                options.Add(option);
            }
            return options;
        }

        private void OpenUseSubMenu(OptionItem optionItem)
        {
            if (!optionItem.OptionData.TryGetValue("itemId", out string itemId))
                return;
            Item item = _inventory?.GetItemStack(itemId)?.Item;
            if (item == null)
                return;
            var useSubMenu = GDEx.Instantiate<UseSubMenu>(UseSubMenu.GetScenePath());
            useSubMenu.Item = item;
            RaiseRequestedAdd(useSubMenu);
        }

        private void UpdateItemDescription(OptionItem optionItem)
        {
            if (!optionItem.OptionData.TryGetValue("itemId", out string itemId))
                return;
            Item item = _inventory?.GetItemStack(itemId)?.Item;
            if (item != null)
            {
                var message = item.Description;
                if (item.Modifiers != null)
                    message += $"\n{item.GetStatDescription()}";
                _itemInfo.UpdateText(message);
            }
        }

        private void UpdateItemList(OptionItem optionItem)
        {
            _inventoryList.ResetContainerFocus();
            ItemType itemType = ItemType.None;
            if (!optionItem.OptionData.TryGetValue("typeName", out string typeName))
                return;
            if (typeName != "All")
                itemType = Enum.Parse<ItemType>(typeName);
            var options = GetItemOptions(itemType);
            _inventoryList.ReplaceChildren(options);
        }
    }
}
