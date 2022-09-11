namespace GameCore.Actors;

public abstract class HealthStateMachineBase : ActorStateMachine<HealthState, HealthStateMachineBase>
{
    protected HealthStateMachineBase(ActorBase actor)
        : base(actor)
    { }
}
