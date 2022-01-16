using System;
using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.JumpStates
{
    public class Fall : State<Player>
    {
        readonly float _fallMultiplier = 2f;
        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            Actor.MotionVelocityY = Math.Min(Actor.MotionVelocity.y + (Actor.JumpGravity * _fallMultiplier * delta), -Actor.JumpVelocity * 1.5f);
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (Actor.IsOnFloor())
            {
                StateMachine.TransitionTo(new Grounded());
            }
        }
    }
}