using System;
using System.Linq;
using GameCore;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Items;

namespace Arenbee.Items;

public partial class HoldItemController : HoldItemControllerBase
{
    private readonly string[] _holdItemIds = { ItemCategoryIds.Weapon };

    public override void Init(ActorBase actor)
    {
        base.Init(actor);
        var slot = Actor.Equipment.GetSlot(EquipmentSlotCategoryIds.Weapon);
        SetHoldItem(null, slot.Item);
    }

    public override void SetHoldItem(ItemBase oldItem, ItemBase newItem)
    {
        if (!_holdItemIds.Contains(oldItem?.ItemCategoryId) && !_holdItemIds.Contains(newItem?.ItemCategoryId))
            return;
        if (oldItem?.ItemCategoryId == ItemCategoryIds.Weapon || newItem?.ItemCategoryId == ItemCategoryIds.Weapon)
        {
            if (oldItem == null && newItem != null)
                Actor.StateController.BaseActionDisabled = true;
            else if (oldItem != null && newItem == null)
                Actor.StateController.BaseActionDisabled = false;
        }
        if (oldItem != null)
            DetachHoldItem(oldItem.Id);
        if (newItem != null)
        {
            var holdItem = GDEx.Instantiate<HoldItem>($"{Config.ItemPath}{newItem.Id}/{newItem.Id}.tscn");
            AttachHoldItem(holdItem);
        }
    }
}
