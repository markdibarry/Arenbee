using System;
using System.Collections.Generic;

namespace GameCore.Items;

public abstract class AEquipmentSlotCategoryDB
{
    protected AEquipmentSlotCategoryDB()
    {
        _categories = BuildDB();
    }

    private readonly EquipmentSlotCategory[] _categories;
    public IReadOnlyCollection<EquipmentSlotCategory> Categories => _categories;

    public EquipmentSlotCategory? GetCategory(string id)
    {
        return Array.Find(_categories, category => category.Id.Equals(id));
    }

    protected abstract EquipmentSlotCategory[] BuildDB();
}
