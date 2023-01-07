namespace GameCore.Actors;

public abstract class MoveStateMachineBase : ActorStateMachine
{
    protected MoveStateMachineBase(ActorState[] states, ActorBase actor) : base(states, actor)
    {
    }
}
