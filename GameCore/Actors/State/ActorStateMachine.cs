using GameCore.Utility;

namespace GameCore.Actors;

public abstract class ActorStateMachine<TState, TStateMachine> : StateMachine<TState, TStateMachine>
    where TState : ActorState<TState, TStateMachine>
    where TStateMachine : ActorStateMachine<TState, TStateMachine>
{
    protected ActorStateMachine(ActorBase actor)
    {
        Actor = actor;
    }

    /// <summary>
    /// The Actor using the StateMachine
    /// </summary>
    /// <value></value>
    public ActorBase Actor { get; set; }
    public IStateController StateController => Actor.StateController;
}
