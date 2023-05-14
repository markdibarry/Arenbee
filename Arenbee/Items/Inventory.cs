using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;

public class Inventory : BaseInventory
{
    public Inventory() { }
    public Inventory(IEnumerable<ItemStack> itemStacks)
        : base(itemStacks)
    { }
}
