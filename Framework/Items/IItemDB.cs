using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public interface IItemDB
    {
        Item GetItem(string id);
        IEnumerable<Item> GetItemsByType(ItemType itemType);
    }
}