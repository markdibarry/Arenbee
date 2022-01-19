using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Idle : State<Actor>
    {
        public override void Enter()
        {
            AnimationName = "Idle";
            StateController.PlayBase(AnimationName);
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
            if (InputHandler.Right.IsActionPressed || InputHandler.Left.IsActionPressed)
            {
                if (InputHandler.Run.IsActionPressed)
                {
                    if (!Actor.IsRunDisabled)
                        StateMachine.TransitionTo(new Run());
                }
                else
                {
                    if (!Actor.IsWalkDisabled)
                        StateMachine.TransitionTo(new Walk());
                }
            }
        }
    }
}