using GameCore.Items;

namespace GameCore.Actors;

public abstract class ActionStateMachineBase : ActorBodyStateMachine
{
    protected ActionStateMachineBase(ActorBodyState[] states, AActorBody actor, HoldItem? holdItem = null)
        : base(states, actor)
    {
        HoldItem = holdItem;
    }

    public HoldItem? HoldItem { get; set; }
}
