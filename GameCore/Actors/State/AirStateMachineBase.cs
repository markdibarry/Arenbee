namespace GameCore.Actors;

public abstract class AirStateMachineBase : ActorStateMachine<AirState, AirStateMachineBase>
{
    protected AirStateMachineBase(ActorBase actor)
        : base(actor)
    { }
}
