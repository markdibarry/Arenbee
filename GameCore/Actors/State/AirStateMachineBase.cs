namespace GameCore.Actors;

public abstract class AirStateMachineBase : ActorBodyStateMachine
{
    protected AirStateMachineBase(AirState[] states, AActorBody actor) : base(states, actor)
    {
    }
}
