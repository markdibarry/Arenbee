using GameCore.Utility;

namespace GameCore.Actors;

public abstract class ActorStateMachine : StateMachine<ActorState>
{
    protected ActorStateMachine(ActorState[] states, ActorBase actor) : base(states)
    {
        Actor = actor;
    }

    /// <summary>
    /// The Actor using the StateMachine
    /// </summary>
    /// <value></value>
    public ActorBase Actor { get; set; }
    public StateControllerBase StateController => Actor.StateController;

    public bool IsBlocked(BlockedState blockedState)
    {
        return State.BlockedStates.HasFlag(blockedState);
    }
}
