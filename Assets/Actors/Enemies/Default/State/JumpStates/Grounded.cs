using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Grounded : JumpState
    {
        public override void Enter()
        {
            Actor.VelocityY = Actor.GroundedGravity;
            StateController.PlayFallbackAnimation();
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (!Actor.IsOnFloor())
                return GetState<Falling>();
            return null;
        }
    }
}