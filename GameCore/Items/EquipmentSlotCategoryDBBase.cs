using System.Collections.Generic;
using System.Linq;

namespace GameCore.Items;

public abstract class EquipmentSlotCategoryDBBase
{
    protected EquipmentSlotCategoryDBBase()
    {
        BuildDB(_categories);
    }

    private readonly List<EquipmentSlotCategoryBase> _categories = new();
    public IEnumerable<EquipmentSlotCategoryBase> Categories => _categories.AsReadOnly();

    protected abstract void BuildDB(List<EquipmentSlotCategoryBase> categories);

    public virtual EquipmentSlotCategoryBase GetCategory(string id)
    {
        return _categories.FirstOrDefault(category => category.Id.Equals(id));
    }
}
