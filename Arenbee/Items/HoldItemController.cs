using System;
using System.Linq;
using GameCore;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Items;

namespace Arenbee.Items;

public partial class HoldItemController : AHoldItemController
{
    private readonly string[] _holdItemIds = { ItemCategoryIds.Weapon, ItemCategoryIds.SubWeapon };

    public override void Init(AActorBody actorBody)
    {
        base.Init(actorBody);
    }

    public override void SetHoldItem(AItem? oldItem, AItem? newItem)
    {
        if (!_holdItemIds.Contains(oldItem?.ItemCategory.Id) && !_holdItemIds.Contains(newItem?.ItemCategory.Id))
            return;
        if (oldItem != null)
            DetachHoldItemByItem(oldItem);
        ActorBody.StateController.BaseActionDisabled = newItem?.ItemCategory.Id == ItemCategoryIds.Weapon;
        if (newItem == null)
            return;
        var holdItem = GDEx.Instantiate<HoldItem>($"{Config.ItemPath}{newItem.Id}/{newItem.Id}.tscn");
        AttachHoldItem(holdItem);
    }
}
