using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Run : State<Player>
    {
        public override void Enter()
        {
            AnimationName = "Run";
            StateController.PlayBase(AnimationName);
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
            if (!Actor.ShouldRun())
            {
                if (Actor.ShouldWalk())
                    StateMachine.TransitionTo(new Walk());
                else
                    StateMachine.TransitionTo(new Idle());
            }
        }
    }
}