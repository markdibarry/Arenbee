namespace Arenbee.Actors;

public abstract class AirState : ActorBodyState
{
    public AirState(ActorBody actor)
        : base(actor) { }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Air");
    }
}
