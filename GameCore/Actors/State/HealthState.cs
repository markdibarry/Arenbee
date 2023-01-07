using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class HealthState : ActorState
{
    protected HealthState(ActorBase actor) : base(actor)
    {
    }

    protected override void PlayAnimation(string animationName)
    {
        StateController.TryPlayAnimation(animationName, "Health");
    }
}
