using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Falling : JumpState
    {
        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            Actor.ApplyFallGravity(delta);
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsOnFloor())
                return GetState<Grounded>();
            return null;
        }
    }
}