using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Run : State<Enemy>
    {
        public override void Enter()
        {
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();

            if (InputHandler.Left.IsActionPressed)
            {
                Actor.MoveX(Facings.Left);
            }
            else if (InputHandler.Right.IsActionPressed)
            {
                Actor.MoveX(Facings.Right);
            }
        }

        public override void Exit()
        {
            Actor.MaxSpeed = Actor.WalkSpeed;
        }

        public override void CheckForTransitions()
        {
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
