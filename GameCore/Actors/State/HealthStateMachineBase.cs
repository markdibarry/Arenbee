namespace GameCore.Actors;

public abstract class HealthStateMachineBase : ActorStateMachine<HealthState, HealthStateMachineBase>
{
    protected HealthStateMachineBase(Actor actor)
        : base(actor)
    { }
}
