using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Walking : MoveState
    {
        public override void Enter()
        {
            Actor.MaxSpeed = Actor.WalkSpeed;
        }

        public override ActorState Update(float delta)
        {
            Actor.UpdateDirection();
            Actor.Move();
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (StateController.IsBlocked(ActorStateType.Move))
                return GetState<Standing>();
            if (Actor.Stats.StatusEffects.HasEffect(StatusEffectType.Burn))
                return GetState<Running>();
            if (InputHandler.GetLeftAxis().x == 0)
                return GetState<Standing>();
            if (InputHandler.Run.IsActionPressed)
                return GetState<Running>();
            return null;
        }
    }
}
