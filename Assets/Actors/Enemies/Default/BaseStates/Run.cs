using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Run : State<Actor>
    {
        public override void Enter() { }

        public override void Update(float delta)
        {
            CheckForTransitions();
            Actor.MaxSpeed = Actor.RunSpeed;
            if (InputHandler.Left.IsActionPressed)
                Actor.MoveX(Facings.Left);
            else if (InputHandler.Right.IsActionPressed)
                Actor.MoveX(Facings.Right);
        }

        public override void Exit() { }

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
