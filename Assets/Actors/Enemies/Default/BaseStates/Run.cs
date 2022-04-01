using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Run : State<Actor>
    {
        public override void Enter()
        {
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            Actor.UpdateDirection();
            Actor.Move();
        }

        public override void Exit() { }

        public override void CheckForTransitions()
        {
            if (Actor.IsRunStuck > 0)
                return;
            if (!InputHandler.Left.IsActionPressed && !InputHandler.Right.IsActionPressed)
            {
                StateMachine.TransitionTo(new Idle());
            }
            else
            {
                if (!InputHandler.Run.IsActionPressed)
                    StateMachine.TransitionTo(new Walk());
            }
        }
    }
}
