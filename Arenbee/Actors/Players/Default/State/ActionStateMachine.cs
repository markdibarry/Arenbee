using GameCore.Actors;
using GameCore.Utility;

namespace Arenbee.Actors.Default.State;

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(AActorBody actorBody)
        : base(
            new ActionState[]
            {
                new NotAttacking(actorBody),
                new UnarmedAttack(actorBody)
            },
            actorBody)
    {
    }

    public class NotAttacking : ActionState
    {
        public NotAttacking(AActorBody actor)
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
            if (StateController.IsBlocked(BlockedState.Attack) || ActorBody.ContextAreas.Count > 0)
                return false;
            if (InputHandler.Attack.IsActionJustPressed)
                return stateMachine.TrySwitchTo<UnarmedAttack>();
            return false;
        }
    }

    public class UnarmedAttack : ActionState
    {
        public UnarmedAttack(AActorBody actorBody)
            : base(actorBody, null)
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
                || ActorBody.AnimationPlayer.CurrentAnimation != AnimationName)
                return stateMachine.TrySwitchTo<NotAttacking>();
            if (InputHandler.Attack.IsActionJustPressed)
                return stateMachine.TrySwitchTo<UnarmedAttack>();
            return false;
        }
    }
}
