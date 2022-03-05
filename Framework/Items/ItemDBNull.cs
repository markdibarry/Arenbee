using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class ItemDBNull : IItemDB
    {
        public Item GetItem(string id) { return null; }
        public IEnumerable<Item> GetItemsByType(ItemType itemType) { return new List<Item>(); }
    }
}
