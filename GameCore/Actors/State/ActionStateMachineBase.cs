using GameCore.Items;

namespace GameCore.Actors;

public abstract class ActionStateMachineBase : ActorStateMachine
{
    protected ActionStateMachineBase(ActorState[] states, ActorBase actor, HoldItem? holdItem = null)
        : base(states, actor)
    {
        HoldItem = holdItem;
    }

    public HoldItem? HoldItem { get; set; }
}
