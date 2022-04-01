using System;

namespace Arenbee.Framework.Items
{
    public class Item
    {
        public Item()
        {
            MaxStack = 1;
            IsUsableInMenu = false;
            IsUsableInField = false;
            IsDroppable = false;
            IsSellable = false;
            IsReusable = false;
        }

        public Item(string id, ItemType itemType) : this()
        {
            Id = id;
            ItemType = itemType;
        }

        public string Id { get; init; }
        public string DisplayName { get; init; }
        public ItemType ItemType { get; init; }
        public string Description { get; init; }
        public string ImgPath { get; init; }
        public int MaxStack { get; init; }
        public bool IsReusable { get; init; }
        public bool IsUsableInMenu { get; init; }
        public bool IsUsableInField { get; init; }
        public bool IsDroppable { get; init; }
        public bool IsSellable { get; init; }
        public int Price { get; init; }
        public Action Use { get; init; }
        public ItemStats ItemStats { get; init; }
    }
}
