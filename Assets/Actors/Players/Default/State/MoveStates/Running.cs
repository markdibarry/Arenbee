using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class Running : MoveState
    {
        public Running() { AnimationName = "Run"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override ActorState Update(float delta)
        {
            Actor.UpdateDirection();
            Actor.Move();
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (StateController.IsBlocked(ActorStateType.Move))
                return GetState<Standing>();
            if (Actor.Stats.HasEffect(StatusEffectType.Burn))
                return null;
            if (InputHandler.GetLeftAxis().x == 0)
                return GetState<Standing>();
            if (InputHandler.Run.IsActionPressed)
                return null;
            return GetState<Walking>();
        }
    }
}