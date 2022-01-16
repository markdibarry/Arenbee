using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;
using Arenbee.Framework.Constants;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Run : State<Player>
    {
        public override void Enter()
        {
            AnimationName = "Run";
            StateController.PlayBase(AnimationName);
        }

        public override void Update(float delta)
        {
            CheckForTransitions();

            if (Input.IsActionPressed(ActionConstants.Right))
            {
                Actor.MoveRight(true);
            }
            else if (Input.IsActionPressed(ActionConstants.Left))
            {
                Actor.MoveLeft(true);
            }
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (!StateMachine.IsActionPressed(Actor.InputHandler.Right) && !StateMachine.IsActionPressed(Actor.InputHandler.Left))
            {
                StateMachine.TransitionTo(new Idle());
            }
            else if (!StateMachine.IsActionPressed(Actor.InputHandler.Run))
            {
                StateMachine.TransitionTo(new Walk());
            }
        }
    }
}