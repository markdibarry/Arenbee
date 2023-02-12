namespace GameCore.Actors;

public abstract class AirState : ActorBodyState
{
    public AirState(AActorBody actor)
        : base(actor) { }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Air");
    }
}
