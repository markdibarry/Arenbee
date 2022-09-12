using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class HealthState : ActorState<HealthState, HealthStateMachineBase>
{
    protected override void PlayAnimation(string animationName)
    {
        StateController.PlayAnimation(animationName, "Health");
    }

    public virtual void HandleDamage(DamageData damageData) { }

    public virtual void HandleHPDepleted() { }
}
