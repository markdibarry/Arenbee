using System.Collections.Generic;

namespace GameCore.Items;

public abstract class ItemCategoryDBBase
{
    protected ItemCategoryDBBase()
    {
        BuildDB(_categories);
    }

    private readonly List<ItemCategoryBase> _categories = new();
    public IEnumerable<ItemCategoryBase> Categories => _categories.AsReadOnly();

    protected abstract void BuildDB(List<ItemCategoryBase> categories);

    public virtual ItemCategoryBase GetType(string id)
    {
        return _categories.Find(itemCategory => itemCategory.Id.Equals(id));
    }
}
