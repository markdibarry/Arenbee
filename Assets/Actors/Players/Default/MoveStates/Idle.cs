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
            if (StateMachine.IsActionPressed(Actor.InputHandler.Right) || StateMachine.IsActionPressed(Actor.InputHandler.Left))
            {
                if (StateMachine.IsActionPressed(Actor.InputHandler.Run))
                {
                    StateMachine.TransitionTo(new Run());
                }
                else
                {
                    StateMachine.TransitionTo(new Walk());
                }
            }
        }
    }
}