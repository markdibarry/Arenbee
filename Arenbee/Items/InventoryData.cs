using System.Collections.Generic;
using System.Linq;
using GameCore.Items;

namespace Arenbee.Items;

public class InventoryData
{
    public InventoryData(AInventory inventory)
    {
        ItemStackData = inventory.Items.Select(x => new ItemStackData(x));
    }

    public InventoryData(IEnumerable<ItemStackData> itemStackData)
    {
        ItemStackData = itemStackData;
    }

    public IEnumerable<ItemStackData> ItemStackData { get; }

    public Inventory CreateInventory()
    {
        IEnumerable<ItemStack> itemStacks = ItemStackData.Select(x => x.CreateItemStack()).OfType<ItemStack>();
        return new(itemStacks);
    }
}
