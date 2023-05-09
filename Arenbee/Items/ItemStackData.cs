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

    public ItemStack? CreateItemStack() => CreateItemStack(s_itemDB);

    public ItemStack? CreateItemStack(IItemDB itemDB)
    {
        AItem? item = s_itemDB.GetItem(ItemId);
        if (item == null)
            return null;
        return new ItemStack(item, Count);
    }
}
