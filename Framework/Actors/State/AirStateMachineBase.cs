namespace Arenbee.Framework.Actors
{
    public abstract class AirStateMachineBase : ActorStateMachine<AirState, AirStateMachineBase>
    {
        protected AirStateMachineBase(Actor actor)
            : base(actor)
        { }
    }
}