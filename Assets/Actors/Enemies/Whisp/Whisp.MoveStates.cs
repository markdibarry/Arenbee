using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        private class Idle : MoveState
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
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return null;
                if (InputHandler.Run.IsActionPressed)
                    return GetState<Running>();
                return GetState<Walking>();
            }
        }

        private class Running : MoveState
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
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return GetState<Idle>();
                else if (!InputHandler.Run.IsActionPressed)
                    return GetState<Walking>();
                return null;
            }
        }

        private class Walking : MoveState
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
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return GetState<Idle>();
                else if (InputHandler.Run.IsActionPressed)
                    return GetState<Running>();
                return null;
            }
        }
    }
}