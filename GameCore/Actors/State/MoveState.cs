namespace GameCore.Actors;

public abstract class MoveState : ActorState
{
    protected MoveState(ActorBase actor) : base(actor)
    {
    }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Move");
    }
}
