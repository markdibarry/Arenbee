using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies.JumpStates
{
    public class Fall : ActorState
    {
        readonly float _fallMultiplier = 2f;

        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            Actor.VelocityY = Actor.Velocity.y.LerpClamp(Actor.JumpGravity * _fallMultiplier, Actor.JumpGravity * delta);
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsOnFloor())
                return new Grounded();
            return null;
        }
    }
}