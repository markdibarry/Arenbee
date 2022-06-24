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
                Locator.GetAudio().PlaySoundFX("menu_close1.wav");
                FocusContainer(_typeList);
                return;
            }
            base.HandleInput(delta);
        }

        protected override void ReplaceDefaultOptions()
        {
            var typeOptions = GetItemTypeOptions();
            _typeList.GridContainer.Columns = typeOptions.Count;
            _typeList.ReplaceChildren(typeOptions);
            _typeList.CurrentIndex = 1;
            _inventoryList.Clear();
            UpdateItemDescription(null);
        }

        protected override void OnItemFocused()
        {
            base.OnItemFocused();
            if (CurrentContainer == _typeList)
                UpdateItemList(CurrentContainer.CurrentItem, resetFocus: true);
            else
                UpdateItemDescription(CurrentContainer.CurrentItem);
        }

        protected override void OnItemSelected()
        {
            base.OnItemSelected();
            if (CurrentContainer == _typeList)
                FocusContainer(_inventoryList);
            else if (CurrentContainer == _inventoryList)
                OpenUseSubMenu(CurrentContainer.CurrentItem);
        }

        public override void ResumeSubMenu()
        {
            UpdateItemList(_typeList.CurrentItem, resetFocus: false);
            base.ResumeSubMenu();
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
            var options = new List<TextOption>();
            foreach (var itemType in Enum<ItemType>.Values())
            {
                var option = textOptionScene.Instantiate<TextOption>();
                option.LabelText = itemType == ItemType.None ? "All" : itemType.Get().Name;
                option.OptionData["itemType"] = itemType;
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

        private void UpdateItemList(OptionItem optionItem, bool resetFocus)
        {
            if (optionItem == null)
                return;
            if (resetFocus)
                _inventoryList.ResetContainerFocus();
            ItemType itemType = optionItem.GetData<ItemType>("itemType");
            List<KeyValueOption> options = GetItemOptions(itemType);
            _inventoryList.ReplaceChildren(options);
        }
    }
}
