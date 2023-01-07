using GameCore.Items;

namespace GameCore.Actors;

public abstract class ActionState : ActorState
{
    protected ActionState(ActorBase actor, HoldItem? holdItem)
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
