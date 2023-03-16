using Arenbee.Items;

namespace Arenbee.Actors;

public abstract class ActionStateMachineBase : ActorBodyStateMachine
{
    protected ActionStateMachineBase(ActorBodyState[] states, ActorBody actor, HoldItem? holdItem = null)
        : base(states, actor)
    {
        HoldItem = holdItem;
    }

    public HoldItem? HoldItem { get; set; }
}
