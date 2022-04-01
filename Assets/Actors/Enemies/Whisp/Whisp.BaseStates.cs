using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        private class Idle : State<Actor>
        {
            public Idle() { AnimationName = "Idle"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override void Update(float delta)
            {
                CheckForTransitions();
            }

            public override void Exit() { }

            public override void CheckForTransitions()
            {
                if (!Actor.IsWalkDisabled && InputHandler.GetLeftAxis() != Godot.Vector2.Zero)
                {
                    if (!Actor.IsRunDisabled && InputHandler.Run.IsActionPressed)
                        StateMachine.TransitionTo(new Run());
                    else
                        StateMachine.TransitionTo(new Walk());
                }
            }
        }

        private class Run : State<Actor>
        {
            public override void Enter()
            {
                Actor.MaxSpeed = Actor.RunSpeed;
            }

            public override void Update(float delta)
            {
                CheckForTransitions();
                Actor.UpdateDirection();
                Actor.Move();
            }

            public override void Exit() { }

            public override void CheckForTransitions()
            {
                if (Actor.IsWalkDisabled || InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    StateMachine.TransitionTo(new Idle());
                else if (Actor.IsRunDisabled || !InputHandler.Run.IsActionPressed)
                    StateMachine.TransitionTo(new Walk());
            }
        }

        private class Walk : State<Actor>
        {
            public override void Enter()
            {
                Actor.MaxSpeed = Actor.WalkSpeed;
            }

            public override void Update(float delta)
            {
                CheckForTransitions();
                Actor.UpdateDirection();
                Actor.Move();
            }

            public override void Exit() { }

            public override void CheckForTransitions()
            {
                if (!Actor.IsWalkDisabled && InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    StateMachine.TransitionTo(new Idle());
                else if (!Actor.IsRunDisabled && InputHandler.Run.IsActionPressed)
                    StateMachine.TransitionTo(new Run());
            }
        }
    }
}