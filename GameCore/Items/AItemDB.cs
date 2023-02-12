﻿using System.Collections.Generic;
using System.Linq;
using System;

namespace GameCore.Items;

public abstract class AItemDB
{
    protected AItemDB(AItemCategoryDB itemCategoryDB)
    {
        _items = BuildDB(itemCategoryDB);
    }

    private readonly AItem[] _items;
    public IReadOnlyCollection<AItem> Items => _items;

    public AItem? GetItem(string id)
    {
        return Array.Find(_items, item => item.Id.Equals(id));
    }

    public IEnumerable<AItem> GetItemsByCategory(string itemCategoryId)
    {
        return _items.Where(item => item.ItemCategoryId.Equals(itemCategoryId));
    }

    protected abstract AItem[] BuildDB(AItemCategoryDB itemCategoryDB);
}