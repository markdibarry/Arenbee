namespace Arenbee.Actors;

public abstract class HealthState : ActorBodyState
{
    protected HealthState(ActorBody actorBody) : base(actorBody)
    {
    }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Health");
    }
}
