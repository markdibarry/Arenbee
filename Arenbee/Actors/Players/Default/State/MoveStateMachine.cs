using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Utility;

namespace Arenbee.Actors.Default.State;

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

    public class Standing : MoveState
    {
        public Standing(ActorBase actor)
            : base(actor)
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

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Move))
                return false;
            if (Actor.Stats.StatusEffects.HasEffect(StatusEffectType.Burn))
                return stateMachine.TrySwitchTo<Running>();
            if (InputHandler.GetLeftAxis().x == 0)
                return false;
            if (InputHandler.Run.IsActionPressed)
                return stateMachine.TrySwitchTo<Running>();
            return stateMachine.TrySwitchTo<Walking>();
        }
    }

    public class Walking : MoveState
    {
        public Walking(ActorBase actor) : base(actor)
        {
            AnimationName = "Walk";
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.WalkSpeed;
        }

        public override void Update(double delta)
        {
            Actor.UpdateDirection();
            Actor.Move();
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Move))
                return stateMachine.TrySwitchTo<Standing>();
            if (Actor.Stats.StatusEffects.HasEffect(StatusEffectType.Burn))
                return stateMachine.TrySwitchTo<Running>();
            if (InputHandler.GetLeftAxis().x == 0)
                return stateMachine.TrySwitchTo<Standing>();
            if (InputHandler.Run.IsActionPressed)
                return stateMachine.TrySwitchTo<Running>();
            return false;
        }
    }

    public class Running : MoveState
    {
        public Running(ActorBase actor) : base(actor)
        {
            AnimationName = "Run";
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override void Update(double delta)
        {
            Actor.UpdateDirection();
            Actor.Move();
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Move))
                return stateMachine.TrySwitchTo<Standing>();
            if (Actor.Stats.HasEffect(StatusEffectType.Burn))
                return false;
            if (InputHandler.GetLeftAxis().x == 0)
                return stateMachine.TrySwitchTo<Standing>();
            if (InputHandler.Run.IsActionPressed)
                return false;
            return stateMachine.TrySwitchTo<Walking>();
        }
    }
}
