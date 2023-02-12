using GameCore.Utility;

namespace GameCore.Actors;

public abstract class ActorBodyStateMachine : StateMachine<ActorBodyState>
{
    protected ActorBodyStateMachine(ActorBodyState[] states, AActorBody actorBody) : base(states)
    {
        ActorBody = actorBody;
    }

    /// <summary>
    /// The Actor using the StateMachine
    /// </summary>
    /// <value></value>
    public AActorBody ActorBody { get; set; }
    public AStateController StateController => ActorBody.StateController;

    public bool IsBlocked(BlockedState blockedState)
    {
        return State.BlockedStates.HasFlag(blockedState);
    }
}
