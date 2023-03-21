using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors;
using GameCore;
using GameCore.Extensions;
using GameCore.Items;
using Godot;

namespace Arenbee.Items;

public partial class HoldItemController : Node2D
{
    private readonly string[] _holdItemIds = { ItemCategoryIds.Weapon, ItemCategoryIds.SubWeapon };
    protected ActorBody ActorBody { get; set; } = null!;
    public List<HoldItem> HoldItems { get; set; } = new();

    protected virtual void AttachHoldItem(HoldItem holdItem)
    {
        if (holdItem == null)
            return;
        holdItem.Init(ActorBody);
        HoldItems.Add(holdItem);
        AddChild(holdItem);
    }

    protected virtual void DetachHoldItemByItem(AItem item)
    {
        HoldItem? holdItem = HoldItems.Find(x => x.Item == item);
        if (holdItem != null)
            DetachHoldItem(holdItem);
    }

    protected virtual void DetachHoldItem(HoldItem holdItem)
    {
        if (!HoldItems.Contains(holdItem))
            return;
        HoldItems.Remove(holdItem);
        holdItem.StateMachine.ExitState();
        RemoveChild(holdItem);
        holdItem.QueueFree();
    }

    public void Init(ActorBody actorBody)
    {
        ActorBody = actorBody;
        if (ActorBody.Actor != null)
        {
            AItem? weapon = ActorBody.Actor.Equipment.GetSlot(EquipmentSlotCategoryIds.Weapon)?.Item;
            if (weapon != null)
                SetHoldItem(null, weapon);
        }
    }

    public void SetHoldItem(AItem? oldItem, AItem? newItem)
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
