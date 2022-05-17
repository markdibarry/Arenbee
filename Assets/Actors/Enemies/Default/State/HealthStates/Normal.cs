using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Normal : HealthState
    {
        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (Actor.Stats.IsKO())
                return GetState<Dead>();
            return null;
        }

        public override void HandleDamage(DamageData damageData)
        {
            bool overDamageThreshold = damageData.TotalDamage > 0 && damageData.ActionType != ActionType.Status;
            Actor.IFrameController.Start(damageData, overDamageThreshold);
            if (overDamageThreshold)
            {
                // Knockback
                Vector2 direction = damageData.SourcePosition.DirectionTo(Actor.GlobalPosition);
                Actor.Velocity = direction * 200;
                StateMachine.TransitionTo<Stagger>(new[] { damageData });
            }
        }
    }
}