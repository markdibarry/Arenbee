using GameCore.Items;

namespace GameCore.Actors;

public abstract class ActionState : ActorBodyState
{
    protected ActionState(AActorBody actor, HoldItem? holdItem)
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
