using GameCore.Actors;

namespace Arenbee.Actors;

public abstract class ActorBodyStateMachine : StateMachine<ActorBodyState>
{
    protected ActorBodyStateMachine(ActorBodyState[] states, ActorBody actorBody) : base(states)
    {
        ActorBody = actorBody;
    }

    /// <summary>
    /// The Actor using the StateMachine
    /// </summary>
    /// <value></value>
    public ActorBody ActorBody { get; set; }
    public StateController StateController => (StateController)ActorBody.StateController;

    public bool IsBlocked(BlockedState blockedState)
    {
        return State.BlockedStates.HasFlag(blockedState);
    }
}
