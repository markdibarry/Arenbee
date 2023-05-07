using Arenbee.Statistics;
using GameCore.Actors;

namespace Arenbee.Actors.State.DefaultEnemy;

public class MoveStateMachine : MoveStateMachineBase
{
    public MoveStateMachine(ActorBody actorBody)
        : base(
            new MoveState[]
            {
                new Standing(actorBody),
                new Walking(actorBody),
                new Running(actorBody)
            },
            actorBody)
    {
    }

    public class Standing : MoveState
    {
        public Standing(ActorBody actorBody) : base(actorBody)
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
            if (StateController.IsBlocked(BlockedState.Move))
                return false;
            if (Stats.HasStatusEffect((int)StatusEffectType.Burn))
                return stateMachine.TrySwitchTo<Running>();
            if (InputHandler.GetLeftAxis().X != 0)
                return stateMachine.TrySwitchTo<Walking>();
            return false;
        }
    }

    public class Walking : MoveState
    {
        public Walking(ActorBody actorBody) : base(actorBody)
        {
            AnimationName = "Walk";
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
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
            if (StateController.IsBlocked(BlockedState.Move))
                return stateMachine.TrySwitchTo<Standing>();
            if (Stats.HasStatusEffect((int)StatusEffectType.Burn))
                return stateMachine.TrySwitchTo<Running>();
            if (InputHandler.GetLeftAxis().X == 0)
                return stateMachine.TrySwitchTo<Standing>();
            if (InputHandler.Run.IsActionPressed)
                return stateMachine.TrySwitchTo<Running>();
            return false;
        }
    }

    public class Running : MoveState
    {
        public Running(ActorBody actorBody) : base(actorBody)
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
            if (StateController.IsBlocked(BlockedState.Move))
                return stateMachine.TrySwitchTo<Standing>();
            if (Stats.HasStatusEffect((int)StatusEffectType.Burn))
                return false;
            if (InputHandler.GetLeftAxis().X == 0)
                return stateMachine.TrySwitchTo<Standing>();
            if (!InputHandler.Run.IsActionPressed)
                return stateMachine.TrySwitchTo<Walking>();
            return false;
        }
    }
}
