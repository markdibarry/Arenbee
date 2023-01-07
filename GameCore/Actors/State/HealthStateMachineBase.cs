namespace GameCore.Actors;

public abstract class HealthStateMachineBase : ActorStateMachine
{
    protected HealthStateMachineBase(ActorState[] states, ActorBase actor) : base(states, actor)
    {
    }
}
