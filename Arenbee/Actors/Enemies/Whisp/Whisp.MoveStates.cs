using GameCore.Actors;
using GameCore.Utility;

namespace Arenbee.Actors.Enemies;

public partial class Whisp : Actor
{
    public class MoveStateMachine : MoveStateMachineBase
    {
        public MoveStateMachine(ActorBase actor)
            : base(
                new MoveState[]
                {
                    new Standing(actor),
                    new Walking(actor),
                    new Running(actor)
                },
                actor)
        {
        }

        private class Standing : MoveState
        {
            public Standing(ActorBase actor) : base(actor)
            {
                AnimationName = "Standing";
            }

            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override void Update(double delta)
            {
            }

            public override void Exit() { }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return false;
                if (InputHandler.Run.IsActionPressed)
                    return stateMachine.TrySwitchTo<Running>();
                return stateMachine.TrySwitchTo<Walking>();
            }
        }

        private class Running : MoveState
        {
            public Running(ActorBase actor) : base(actor)
            {
            }

            public override void Enter()
            {
                Actor.MaxSpeed = Actor.RunSpeed;
            }

            public override void Update(double delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
            }

            public override void Exit() { }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return stateMachine.TrySwitchTo<Standing>();
                else if (!InputHandler.Run.IsActionPressed)
                    return stateMachine.TrySwitchTo<Walking>();
                return false;
            }
        }

        private class Walking : MoveState
        {
            public Walking(ActorBase actor) : base(actor)
            {
            }

            public override void Enter()
            {
                Actor.MaxSpeed = Actor.WalkSpeed;
            }

            public override void Update(double delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
            }

            public override void Exit() { }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (InputHandler.GetLeftAxis() == Godot.Vector2.Zero)
                    return stateMachine.TrySwitchTo<Standing>();
                else if (InputHandler.Run.IsActionPressed)
                    return stateMachine.TrySwitchTo<Running>();
                return false;
            }
        }
    }
}
