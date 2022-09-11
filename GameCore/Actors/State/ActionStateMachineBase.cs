using GameCore.Items;

namespace GameCore.Actors;

public abstract class ActionStateMachineBase : ActorStateMachine<ActionState, ActionStateMachineBase>
{
    protected ActionStateMachineBase(ActorBase actor, HoldItem holdItem = null)
        : base(actor)
    {
        HoldItem = holdItem;
    }

    public HoldItem HoldItem { get; set; }
}
