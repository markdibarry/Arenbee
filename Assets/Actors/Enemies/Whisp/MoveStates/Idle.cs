using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.WhispNS.MoveStates
{
    public class Idle : State<Enemy>
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
            if (InputHandler.GetLeftAxis() != Godot.Vector2.Zero)
            {
                if (InputHandler.Run.IsActionPressed)
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