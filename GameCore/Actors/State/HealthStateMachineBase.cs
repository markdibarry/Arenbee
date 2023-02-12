namespace GameCore.Actors;

public abstract class HealthStateMachineBase : ActorBodyStateMachine
{
    protected HealthStateMachineBase(ActorBodyState[] states, AActorBody actor) : base(states, actor)
    {
    }
}
