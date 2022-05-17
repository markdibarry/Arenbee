using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class Standing : MoveState
    {
        public Standing() { AnimationName = "Idle"; }
        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (StateController.IsBlocked(ActorStateType.Move))
                return null;
            if (Actor.Stats.StatusEffects.HasEffect(StatusEffectType.Burn))
                return GetState<Running>();
            if (InputHandler.GetLeftAxis().x == 0)
                return null;
            if (InputHandler.Run.IsActionPressed)
                return GetState<Running>();
            return GetState<Walking>();
        }
    }
}