using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies.JumpStates
{
    public class Fall : State<Actor>
    {
        readonly float _fallMultiplier = 2f;

        public override void Enter() { }

        public override void Update(float delta)
        {
            CheckForTransitions();
            Actor.VelocityY = Actor.Velocity.y.LerpClamp(Actor.JumpGravity * _fallMultiplier, Actor.JumpGravity * delta);
        }

        public override void Exit() { }

        public override void CheckForTransitions()
        {
            if (Actor.IsOnFloor())
                StateMachine.TransitionTo(new Grounded());
        }
    }
}