namespace Arenbee.Framework.Actors
{
    public abstract class JumpState : ActorState
    {
        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayJumpAnimation(animationName);
        }
    }
}