namespace GameCore.Actors;

public abstract class MoveStateMachineBase : ActorStateMachine<MoveState, MoveStateMachineBase>
{
    protected MoveStateMachineBase(ActorBase actor)
        : base(actor)
    { }
}
