using Arenbee.Framework;
using Arenbee.Framework.Actors;

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

            if (StateMachine.IsActionPressed(Actor.InputHandler.Right))
            {
                Actor.MoveRight();
            }
            else if (StateMachine.IsActionPressed(Actor.InputHandler.Left))
            {
                Actor.MoveLeft();
            }
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (!StateMachine.IsActionPressed(Actor.InputHandler.Left) && !StateMachine.IsActionPressed(Actor.InputHandler.Right))
            {
                StateMachine.TransitionTo(new Idle());
            }
            else if (StateMachine.IsActionPressed(Actor.InputHandler.Run))
            {
                StateMachine.TransitionTo(new Run());
            }
        }
    }
}