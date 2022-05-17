using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Standing : MoveState
    {
        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (StateController.IsBlocked(ActorStateType.Move))
                return null;
            if (Actor.Stats.HasEffect(StatusEffectType.Burn))
                return GetState<Running>();
            if (Actor.InputHandler.GetLeftAxis().x != 0)
                return GetState<Walking>();
            return null;
        }
    }
}