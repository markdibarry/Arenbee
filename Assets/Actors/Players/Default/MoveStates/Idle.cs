using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Idle : State<Player>
    {
        public override void Enter()
        {
            StateController.PlayBase("Idle");
        }

        public override void Update()
        {
            CheckForTransitions();
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
                else if (Input.IsActionPressed(ActionConstants.Right) || Input.IsActionPressed(ActionConstants.Left))
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