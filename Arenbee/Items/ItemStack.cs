using GameCore.Items;

namespace Arenbee.Items;
public class ItemStack : AItemStack
{
    public ItemStack(AItem item, int amount)
        : base(item, amount)
    {
    }
}
