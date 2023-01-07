namespace GameCore.Actors;

public abstract class AirStateMachineBase : ActorStateMachine
{
    protected AirStateMachineBase(AirState[] states, ActorBase actor) : base(states, actor)
    {
    }
}
