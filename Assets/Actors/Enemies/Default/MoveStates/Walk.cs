using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Walk : State<Actor>
    {
        public override void Enter()
        {
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
        }

        public override void CheckForTransitions()
        {
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
