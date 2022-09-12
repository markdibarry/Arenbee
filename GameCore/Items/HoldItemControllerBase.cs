using System.Collections.Generic;
using GameCore.Actors;
using Godot;

namespace GameCore.Items;

public abstract partial class HoldItemControllerBase : Node2D
{
    protected ActorBase Actor { get; set; }
    public List<HoldItem> HoldItems { get; set; } = new();

    public virtual void Init(ActorBase actor)
    {
        Actor = actor;
    }

    public abstract void SetHoldItem(ItemBase oldItem, ItemBase newItem);

    protected virtual void AttachHoldItem(HoldItem holdItem)
    {
        if (holdItem == null)
            return;
        holdItem.Init(Actor);
        HoldItems.Add(holdItem);
        AddChild(holdItem);
    }

    protected virtual void DetachHoldItem(string itemId)
    {
        HoldItem holdItem = HoldItems.Find(x => x.ItemId == itemId);
        DetachHoldItem(holdItem);
    }

    protected virtual void DetachHoldItem(HoldItem holdItem)
    {
        if (holdItem == null)
            return;
        HoldItems.Remove(holdItem);
        holdItem.StateMachine.ExitState();
        RemoveChild(holdItem);
        holdItem.QueueFree();
    }
}
