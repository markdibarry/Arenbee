namespace Arenbee.Actors;

public abstract class MoveStateMachineBase : ActorBodyStateMachine
{
    protected MoveStateMachineBase(ActorBodyState[] states, ActorBody actor) : base(states, actor)
    {
    }
}
