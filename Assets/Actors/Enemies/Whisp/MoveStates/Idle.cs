using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.WhispNS.BaseStates
{
    public class Idle : State<Actor>
    {
        public Idle() { AnimationName = "Idle"; }
        public override void Enter()
        {
            StateMachine.PlayAnimation(AnimationName);
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
            if (!Actor.IsWalkDisabled && InputHandler.GetLeftAxis() != Godot.Vector2.Zero)
            {
                if (!Actor.IsRunDisabled && InputHandler.Run.IsActionPressed)
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