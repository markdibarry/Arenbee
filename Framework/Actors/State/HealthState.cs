using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Actors
{
    public abstract class HealthState : ActorState
    {
        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayHealthAnimation(animationName);
        }

        public virtual void HandleDamage(DamageData damageData) { }
    }
}