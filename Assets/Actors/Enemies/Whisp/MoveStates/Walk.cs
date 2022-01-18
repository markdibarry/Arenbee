using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.WhispNS.MoveStates
{
    public class Walk : State<Enemy>
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
            if (!InputHandler.Left.IsActionPressed
                && !InputHandler.Right.IsActionPressed
                && !InputHandler.Up.IsActionPressed
                && !InputHandler.Down.IsActionPressed)
            {
                StateMachine.TransitionTo(new Idle());
            }
            else if (InputHandler.Run.IsActionPressed)
            {
                StateMachine.TransitionTo(new Run());
            }
        }
    }
}