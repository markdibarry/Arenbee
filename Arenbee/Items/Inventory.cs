using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;

public class Inventory : AInventory
{
    public Inventory() { }
    public Inventory(IEnumerable<AItemStack> itemStacks)
        : base(itemStacks)
    { }
}
