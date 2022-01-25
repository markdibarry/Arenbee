using System;
using Arenbee.Framework.Enums;

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

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public ItemType ItemType { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }
        public int MaxStack { get; set; }
        public bool IsReusable { get; set; }
        public bool IsUsableInMenu { get; set; }
        public bool IsUsableInField { get; set; }
        public bool IsDroppable { get; set; }
        public bool IsSellable { get; set; }
        public int Price { get; set; }
        public Action Use { get; set; }
        public ItemStats ItemStats { get; set; }
    }
}
