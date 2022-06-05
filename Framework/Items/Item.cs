﻿using System.Collections.Generic;
using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Items
{
    public class Item
    {
        public Item()
        {
            MaxStack = 1;
            IsDroppable = false;
            IsSellable = false;
            IsReusable = false;
            UseData = new ItemUseData();
        }

        public Item(string id, ItemType itemType)
            : this()
        {
            Id = id;
            ItemType = itemType;
        }

        public string Id { get; init; }
        public string Description { get; init; }
        public string DisplayName { get; init; }
        public string ImgPath { get; init; }
        public bool IsDroppable { get; init; }
        public bool IsReusable { get; init; }
        public bool IsSellable { get; init; }
        public ItemType ItemType { get; init; }
        public int MaxStack { get; init; }
        public ICollection<Modifier> Modifiers { get; set; }
        public int Price { get; init; }
        public ItemUseData UseData { get; init; }

        public void AddToStats(Stats stats)
        {
            if (Modifiers.Count == 0) return;
            foreach (Modifier mod in Modifiers)
                stats.AddMod(mod);
        }

        public void RemoveFromStats(Stats stats)
        {
            if (Modifiers.Count == 0) return;
            foreach (Modifier mod in Modifiers)
                stats.RemoveMod(mod);
        }
    }
}
