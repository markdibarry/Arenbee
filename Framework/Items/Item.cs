using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors.Stats;
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
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public ItemType ItemType { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }
        public int MaxStack { get; set; }
        public bool IsUsableInMenu { get; set; }
        public bool IsUsableInField { get; set; }
        public bool IsDroppable { get; set; }
        public bool IsSellable { get; set; }
        public int Price { get; set; }
        public Action Use { get; set; }
    }
}