using GameCore.Actors;
using GameCore.Utility;

namespace Arenbee.Actors.Default.State;

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(ActorBase actor)
        : base(
            new ActionState[]
            {
                new NotAttacking(actor),
                new UnarmedAttack(actor)
            },
            actor)
    {
    }

    public class NotAttacking : ActionState
    {
        public NotAttacking(ActorBase actor)
            : base(actor, null)
        { }

        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override void Update(double delta)
        {
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Attack) || Actor.ContextAreas.Count > 0)
                return false;
            if (InputHandler.Attack.IsActionJustPressed)
                return stateMachine.TrySwitchTo<UnarmedAttack>();
            return false;
        }
    }

    public class UnarmedAttack : ActionState
    {
        public UnarmedAttack(ActorBase actor)
            : base(actor, null)
        {
            AnimationName = "UnarmedAttack";
            BlockedStates = BlockedState.Jumping | BlockedState.Move;
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
            if (StateController.IsBlocked(BlockedState.Attack)
                || Actor.AnimationPlayer.CurrentAnimation != AnimationName)
                return stateMachine.TrySwitchTo<NotAttacking>();
            if (InputHandler.Attack.IsActionJustPressed)
                return stateMachine.TrySwitchTo<UnarmedAttack>();
            return false;
        }
    }
}
