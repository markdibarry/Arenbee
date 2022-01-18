using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Walk : State<Player>
    {
        public override void Enter()
        {
            AnimationName = "Walk";
            StateController.PlayBase(AnimationName);
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
            if (Actor.ShouldWalk())
            {
                if (Actor.ShouldRun())
                    StateMachine.TransitionTo(new Run());
            }
            else
            {
                StateMachine.TransitionTo(new Idle());
            }
        }
    }
}