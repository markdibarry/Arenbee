using System;
using System.Collections.Generic;
using GameCore.Statistics;

namespace GameCore.Items;

public class AItem : IEquatable<AItem>
{
    protected AItem()
    {
        MaxStack = 1;
        IsDroppable = false;
        IsSellable = false;
        IsReusable = false;
        UseData = new ItemUseData();
    }

    public AItem(string id, ItemCategory itemCategory)
        : this()
    {
        Id = id;
        ItemCategory = itemCategory;
    }

    public string Id { get; init; }
    public string Description { get; init; }
    public string DisplayName { get; init; }
    public string ImgPath { get; init; }
    public bool IsDroppable { get; init; }
    public bool IsReusable { get; init; }
    public bool IsSellable { get; init; }
    public ItemCategory ItemCategory { get; }
    public int MaxStack { get; init; }
    public ICollection<Modifier> Modifiers { get; set; }
    public int Price { get; init; }
    public ItemUseData UseData { get; init; }

    public void AddToStats(Stats stats)
    {
        if (Modifiers.Count == 0)
            return;
        foreach (Modifier mod in Modifiers)
            stats.AddMod(mod);
    }

    public bool Equals(AItem? other)
    {
        if (other == null)
            return false;
        return other.Id == Id;
    }

    public void RemoveFromStats(Stats stats)
    {
        if (Modifiers.Count == 0)
            return;
        foreach (Modifier mod in Modifiers)
            stats.RemoveMod(mod);
    }
}
