namespace Arenbee.Framework.Actors
{
    public abstract class MoveStateMachineBase : ActorStateMachine<MoveState, MoveStateMachineBase>
    {
        protected MoveStateMachineBase(Actor actor)
            : base(actor)
        { }
    }
}