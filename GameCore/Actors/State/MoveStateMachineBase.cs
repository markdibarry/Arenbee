namespace GameCore.Actors;

public abstract class MoveStateMachineBase : ActorBodyStateMachine
{
    protected MoveStateMachineBase(ActorBodyState[] states, AActorBody actor) : base(states, actor)
    {
    }
}
