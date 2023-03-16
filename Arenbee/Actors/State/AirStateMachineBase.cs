namespace Arenbee.Actors;

public abstract class AirStateMachineBase : ActorBodyStateMachine
{
    protected AirStateMachineBase(AirState[] states, ActorBody actor) : base(states, actor)
    {
    }
}
