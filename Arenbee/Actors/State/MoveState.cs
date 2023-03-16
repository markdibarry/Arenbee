namespace Arenbee.Actors;

public abstract class MoveState : ActorBodyState
{
    protected MoveState(ActorBody actor) : base(actor)
    {
    }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Move");
    }
}
