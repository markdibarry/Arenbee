using System.Text.Json.Serialization;
using GameCore.Items;

namespace Arenbee.Items;

public class ItemStackData
{
    private static readonly IItemDB s_itemDB = ItemsLocator.ItemDB;

    public ItemStackData(ItemStackData itemStackData)
        : this(itemStackData.ItemId, itemStackData.Count)
    {
    }

    public ItemStackData(ItemStack itemStack)
        : this(itemStack.Item.Id, itemStack.Count)
    {
    }

    [JsonConstructor]
    public ItemStackData(string itemId, int count)
    {
        Count = count;
        ItemId = itemId;
    }

    public int Count { get; }
    public string ItemId { get; }

    public ItemStack? ToItemStack()
    {
        BaseItem? item = s_itemDB.GetItem(ItemId);
        return item == null ? null : new ItemStack(item, Count);
    }
}
