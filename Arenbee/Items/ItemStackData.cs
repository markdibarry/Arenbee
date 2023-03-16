using GameCore.Items;
using GameCore.Utility;

namespace Arenbee.Items;

public class ItemStackData
{
    private static readonly AItemDB s_itemDB = Locator.ItemDB;

    public ItemStackData(ItemStackData itemStackData)
        : this(itemStackData.ItemId, itemStackData.Count)
    {
    }

    public ItemStackData(ItemStack itemStack)
        : this(itemStack.Item.Id, itemStack.Count)
    {
    }

    public ItemStackData(string itemId, int count)
    {
        Count = count;
        ItemId = itemId;
    }

    public int Count { get; }
    public string ItemId { get; }

    public ItemStack? CreateItemStack() => CreateItemStack(s_itemDB);

    public ItemStack? CreateItemStack(AItemDB itemDB)
    {
        AItem? item = s_itemDB.GetItem(ItemId);
        if (item == null)
            return null;
        return new ItemStack(item, Count);
    }
}
