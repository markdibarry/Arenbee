using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        private class Idle : ActorState
        {
            public Idle() { AnimationName = "Idle"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override ActorState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActorState CheckForTransitions()
            {
                if (!Actor.IsWalkDisabled && InputHandler.GetLeftAxis() != Godot.Vector2.Zero)
                {
                    if (!Actor.IsRunDisabled && InputHandler.Run.IsActionPressed)
                        return new Run();
                    return new Walk();
                }
                return null;
            }
        }

        private class Run : ActorState
        {
            public override void Enter()
            {
                Actor.MaxSpeed = Actor.RunSpeed;
            }

            public override ActorState Update(float delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActorState CheckForTransitions()
            {
                if (Actor.IsWalkDisabled || InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return new Idle();
                else if (Actor.IsRunDisabled || !InputHandler.Run.IsActionPressed)
                    return new Walk();
                return null;
            }
        }

        private class Walk : ActorState
        {
            public override void Enter()
            {
                Actor.MaxSpeed = Actor.WalkSpeed;
            }

            public override ActorState Update(float delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActorState CheckForTransitions()
            {
                if (!Actor.IsWalkDisabled && InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return new Idle();
                else if (!Actor.IsRunDisabled && InputHandler.Run.IsActionPressed)
                    return new Run();
                return null;
            }
        }
    }
}