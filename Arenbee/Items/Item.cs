using GameCore.Items;

namespace Arenbee.Items;

public class Item : AItem
{
    public Item(string id, ItemCategory itemCategory)
        : base(id, itemCategory)
    {
    }
}

public static class ItemIds
{
    public const string HockeyStick = "HockeyStick";
}
