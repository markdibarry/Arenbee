namespace Arenbee.Actors;

public abstract class HealthStateMachineBase : ActorBodyStateMachine
{
    protected HealthStateMachineBase(ActorBodyState[] states, ActorBody actor) : base(states, actor)
    {
    }
}
