namespace GameCore.Actors;

public abstract class AirState : ActorState
{
    public AirState(ActorBase actor)
        : base(actor) { }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Air");
    }
}
