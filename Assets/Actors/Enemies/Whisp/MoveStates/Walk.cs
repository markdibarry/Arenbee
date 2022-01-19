using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.WhispNS.MoveStates
{
    public class Walk : State<Actor>
    {
        public override void Enter()
        {
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
            if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
            {
                StateMachine.TransitionTo(new Idle());
            }
            else if (InputHandler.Run.IsActionPressed)
            {
                StateMachine.TransitionTo(new Run());
            }
        }
    }
}