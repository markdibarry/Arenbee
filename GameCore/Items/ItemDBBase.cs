using System.Collections.Generic;
using System.Linq;

namespace GameCore.Items;

public abstract class ItemDBBase
{
    protected ItemDBBase()
    {
        BuildDB(_items);
    }

    private readonly List<ItemBase> _items = new();
    public IEnumerable<ItemBase> Items => _items.AsReadOnly();

    protected abstract void BuildDB(List<ItemBase> items);

    public virtual ItemBase GetItem(string id)
    {
        return _items.Find(item => item.Id.Equals(id));
    }

    public IEnumerable<ItemBase> GetItemsByCategory(string itemCategoryId)
    {
        return _items.Where(item => item.ItemCategoryId.Equals(itemCategoryId));
    }
}
