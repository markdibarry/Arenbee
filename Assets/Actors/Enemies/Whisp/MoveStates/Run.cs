using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.WhispNS.BaseStates
{
    public class Run : State<Actor>
    {
        public override void Enter()
        {
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            Actor.MaxSpeed = Actor.WalkSpeed;
        }

        public override void CheckForTransitions()
        {
            if (Actor.IsWalkDisabled || InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
            {
                StateMachine.TransitionTo(new Idle());
            }
            else if (Actor.IsRunDisabled || !InputHandler.Run.IsActionPressed)
            {
                StateMachine.TransitionTo(new Walk());
            }
        }
    }
}