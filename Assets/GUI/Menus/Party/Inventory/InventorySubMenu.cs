using System;
using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class InventorySubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private DynamicTextContainer _itemInfo;
        private OptionContainer _inventoryList;
        private OptionContainer _typeList;
        private Inventory _inventory;

        protected override void CustomOptionsSetup()
        {
            _inventory = GameRoot.Instance.CurrentGame.Party.Inventory;
            _inventoryList?.ReplaceChildren(GetItemOptions(null));

            if (_typeList != null)
            {
                var typeOptions = GetItemTypeOptions();
                _typeList.GridContainer.Columns = typeOptions.Count;
                _typeList.ReplaceChildren(typeOptions);
            }
            base.CustomOptionsSetup();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (optionContainer == _typeList)
            {
                ItemType? itemType = null;
                if (!optionItem.OptionData.TryGetValue("typeName", out string typeName))
                    return;
                if (typeName != "All")
                    itemType = Enum.Parse<ItemType>(typeName);
                var options = GetItemOptions(itemType);
                _inventoryList.ReplaceChildren(options);
                _inventoryList.InitItems();
            }
            else
            {
                if (!optionItem.OptionData.TryGetValue("itemId", out string itemId))
                    return;
                Item item = _inventory.GetItemStack(itemId)?.Item;
                if (item != null)
                {
                    var message = item.Description;
                    if (item.ItemStats != null)
                        message += $"\n{item.ItemStats.GetStatDescription()}";
                    _itemInfo.UpdateText(message);
                }
            }
        }

        protected override void OnFocusOOB(OptionContainer containerLeavingFocus, Direction direction)
        {
            if (containerLeavingFocus == _typeList)
                HandleTypeListOOB(containerLeavingFocus, direction);
            else
                HandleInventoryListOOB(containerLeavingFocus, direction);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _typeList = Foreground.GetNode<OptionContainer>("TypeList");
            OptionContainers.Add(_typeList);
            _inventoryList = Foreground.GetNode<OptionContainer>("InventoryList");
            OptionContainers.Add(_inventoryList);
            _itemInfo = Foreground.GetNode<DynamicTextContainer>("ItemInfo");
        }

        private List<KeyValueOption> GetItemOptions(ItemType? itemType)
        {
            var keyValueOptionScene = GD.Load<PackedScene>(KeyValueOption.GetScenePath());
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
                option.OptionData.Add("itemId", itemStack.ItemId);
                options.Add(option);
            }
            return options;
        }

        private List<TextOption> GetItemTypeOptions()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            var allOption = textOptionScene.Instantiate<TextOption>();
            allOption.LabelText = "All";
            allOption.OptionData.Add("typeName", "All");
            var options = new List<TextOption>() { allOption };
            foreach (var typeName in Enum.GetNames(typeof(ItemType)))
            {
                var option = textOptionScene.Instantiate<TextOption>();
                option.LabelText = typeName;
                option.OptionData.Add("typeName", typeName);
                options.Add(option);
            }
            return options;
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
                _itemInfo.UpdateText(string.Empty);
                FocusContainerPreviousItem(_typeList);
            }
            else
            {
                base.OnFocusOOB(container, direction);
            }
        }
    }
}
