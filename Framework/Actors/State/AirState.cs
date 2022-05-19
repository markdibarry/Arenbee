namespace Arenbee.Framework.Actors
{
    public abstract class AirState : ActorState<AirState, AirStateMachineBase>
    {
        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayAirAnimation(animationName);
        }
    }
}