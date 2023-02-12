namespace GameCore.Actors;

public abstract class MoveState : ActorBodyState
{
    protected MoveState(AActorBody actor) : base(actor)
    {
    }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Move");
    }
}
