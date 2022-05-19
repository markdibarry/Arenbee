namespace Arenbee.Framework.Actors
{
    public abstract class MoveState : ActorState<MoveState, MoveStateMachineBase>
    {
        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayMoveAnimation(animationName);
        }
    }
}