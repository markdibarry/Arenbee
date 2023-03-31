using GameCore.Utility;

namespace Arenbee.Actors.ActorBodies;

public partial class Whisp : ActorBody
{
    public class MoveStateMachine : MoveStateMachineBase
    {
        public MoveStateMachine(ActorBody actor)
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
            public Standing(ActorBody actor) : base(actor)
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
            public Running(ActorBody actor) : base(actor)
            {
            }

            public override void Enter()
            {
                ActorBody.MaxSpeed = ActorBody.RunSpeed;
            }

            public override void Update(double delta)
            {
                ActorBody.UpdateDirection();
                ActorBody.Move();
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
            public Walking(ActorBody actor) : base(actor)
            {
            }

            public override void Enter()
            {
                ActorBody.MaxSpeed = ActorBody.WalkSpeed;
            }

            public override void Update(double delta)
            {
                ActorBody.UpdateDirection();
                ActorBody.Move();
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
