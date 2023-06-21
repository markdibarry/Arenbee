using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GameCore.Items;

namespace Arenbee.Items;

public class InventoryData
{
    public InventoryData(BaseInventory inventory)
    {
        ItemStackData = inventory.Items.Select(x => new ItemStackData(x));
    }

    [JsonConstructor]
    public InventoryData(IEnumerable<ItemStackData> itemStackData)
    {
        ItemStackData = itemStackData;
    }

    public IEnumerable<ItemStackData> ItemStackData { get; }

    public Inventory CreateInventory()
    {
        IEnumerable<ItemStack> itemStacks = ItemStackData.Select(x => x.ToItemStack()).OfType<ItemStack>();
        return new(itemStacks);
    }
}
