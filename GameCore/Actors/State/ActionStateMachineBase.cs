namespace GameCore.Actors
{
    public abstract class ActionStateMachineBase : ActorStateMachine<ActionState, ActionStateMachineBase>
    {
        protected ActionStateMachineBase(Actor actor)
            : base(actor)
        { }
    }
}