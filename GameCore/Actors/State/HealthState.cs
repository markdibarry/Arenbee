using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class HealthState : ActorState<HealthState, HealthStateMachineBase>
{
    protected override void PlayAnimation(string animationName)
    {
        StateController.PlayHealthAnimation(animationName);
    }

    public virtual void HandleDamage(DamageData damageData) { }

    public virtual void HandleHPDepleted() { }
}
