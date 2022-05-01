using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.JumpStates
{
    public class Grounded : ActorState
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
                return new Fall();
            return null;
        }
    }
}