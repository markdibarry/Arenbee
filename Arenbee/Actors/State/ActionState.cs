using Arenbee.Items;

namespace Arenbee.Actors;

public abstract class ActionState : ActorBodyState
{
    protected ActionState(ActorBody actor, HoldItem? holdItem)
        : base(actor)
    {
        HoldItem = holdItem;
    }

    public HoldItem? HoldItem { get; set; }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Action", HoldItem);
    }
}
