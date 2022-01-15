using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;
using Arenbee.Framework.Constants;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Walk : State<Player>
    {
        public override void Enter()
        {
            StateController.PlayBase("Walk");
        }

        public override void Update()
        {
            CheckForTransitions();

            if (Input.IsActionPressed(ActionConstants.Right))
            {
                Actor.MoveRight();
            }
            else if (Input.IsActionPressed(ActionConstants.Left))
            {
                Actor.MoveLeft();
            }
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (Actor.IsOnFloor())
            {
                if (Input.IsActionJustPressed(ActionConstants.Jump))
                {
                    StateMachine.TransitionTo(new Jump());
                }
                else if (!Input.IsActionPressed(ActionConstants.Left) && !Input.IsActionPressed(ActionConstants.Right))
                {
                    StateMachine.TransitionTo(new Idle());
                }
                else if (Input.IsActionPressed(ActionConstants.Run))
                {
                    StateMachine.TransitionTo(new Run());
                }
            }
            else
            {
                if (Actor.MotionVelocity.y >= 0)
                {
                    StateMachine.TransitionTo(new Fall());
                }
            }
        }
    }
}