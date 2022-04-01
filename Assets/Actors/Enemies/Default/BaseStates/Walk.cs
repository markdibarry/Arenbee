using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Walk : State<Actor>
    {
        public override void Enter()
        {
            Actor.MaxSpeed = Actor.WalkSpeed;
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
            {
                StateMachine.TransitionTo(new Run());
                return;
            }

            if (InputHandler.Left.IsActionPressed || InputHandler.Right.IsActionPressed)
            {
                if (InputHandler.Run.IsActionPressed)
                    StateMachine.TransitionTo(new Run());
            }
            else
            {
                StateMachine.TransitionTo(new Idle());
            }
        }
    }
}
