namespace Arenbee.Framework.Actors
{
    public abstract class MoveState : ActorState
    {
        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayMoveAnimation(animationName);
        }
    }
}