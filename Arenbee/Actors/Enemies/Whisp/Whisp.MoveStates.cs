using GameCore.Actors;

namespace Arenbee.Actors.Enemies;

public partial class Whisp : Actor
{
    public class MoveStateMachine : MoveStateMachineBase
    {
        public MoveStateMachine(ActorBase actor)
            : base(actor)
        {
            AddState<Standing>();
            AddState<Walking>();
            AddState<Running>();
            InitStates(this);
        }

        private class Standing : MoveState
        {
            public Standing() { AnimationName = "Standing"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override MoveState Update(double delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override MoveState CheckForTransitions()
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

            public override MoveState Update(double delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override MoveState CheckForTransitions()
            {
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return GetState<Standing>();
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

            public override MoveState Update(double delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override MoveState CheckForTransitions()
            {
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return GetState<Standing>();
                else if (InputHandler.Run.IsActionPressed)
                    return GetState<Running>();
                return null;
            }
        }
    }
}
