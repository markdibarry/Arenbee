using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Idle : State<Actor>
    {
        public override void Enter() { }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit() { }

        public override void CheckForTransitions()
        {
            if (Actor.IsRunStuck > 0)
            {
                StateMachine.TransitionTo(new Run());
                return;
            }
            if (InputHandler.Left.IsActionPressed || InputHandler.Right.IsActionPressed)
                StateMachine.TransitionTo(new Walk());
        }
    }
}