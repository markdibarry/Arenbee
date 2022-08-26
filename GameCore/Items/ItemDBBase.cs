using System.Collections.Generic;
using System.Linq;

namespace GameCore.Items
{
    public abstract class ItemDBBase
    {
        protected List<Item> Items { get; } = new();

        public virtual Item GetItem(string id)
        {
            return Items.Find(item => item.Id.Equals(id));
        }

        public virtual IEnumerable<Item> GetItemsByType(ItemType itemType)
        {
            return Items.Where(item => item.ItemType.Equals(itemType));
        }
    }
}
