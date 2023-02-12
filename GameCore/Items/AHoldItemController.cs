using System.Collections.Generic;
using GameCore.Actors;
using Godot;

namespace GameCore.Items;

public abstract partial class AHoldItemController : Node2D
{
    protected AActorBody ActorBody { get; set; } = null!;
    public List<HoldItem> HoldItems { get; set; } = new();

    public virtual void Init(AActorBody actorBody)
    {
        ActorBody = actorBody;
    }

    public abstract void SetHoldItem(AItem? oldItem, AItem? newItem);

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
}
