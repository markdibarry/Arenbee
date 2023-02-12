namespace GameCore.Actors;

public abstract class HealthState : ActorBodyState
{
    protected HealthState(AActorBody actorBody) : base(actorBody)
    {
    }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Health");
    }
}
