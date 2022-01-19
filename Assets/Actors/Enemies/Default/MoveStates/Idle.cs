using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Idle : State<Actor>
    {
        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (InputHandler.Left.IsActionPressed || InputHandler.Right.IsActionPressed)
            {
                StateMachine.TransitionTo(new Walk());
            }
        }
    }
}