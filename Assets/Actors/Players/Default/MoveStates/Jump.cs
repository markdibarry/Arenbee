using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Jump : State<Player>
    {
        public override void Enter()
        {
            Actor.Jump();
            StateController.PlayBase("Jump");
        }

        public override void Update()
        {
            CheckForTransitions();

            if (Input.IsActionPressed(ActionConstants.Right))
            {
                Actor.MoveRight(Input.IsActionPressed(ActionConstants.Run));
            }
            else if (Input.IsActionPressed(ActionConstants.Left))
            {
                Actor.MoveLeft(Input.IsActionPressed(ActionConstants.Run));
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
                else if (Input.IsActionPressed(ActionConstants.Left) || Input.IsActionPressed(ActionConstants.Right))
                {
                    if (Input.IsActionPressed(ActionConstants.Run))
                    {
                        StateMachine.TransitionTo(new Run());
                    }
                    else
                    {
                        StateMachine.TransitionTo(new Walk());
                    }
                }
                else
                {
                    StateMachine.TransitionTo(new Idle());
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