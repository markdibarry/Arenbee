using GameCore.Actors;

namespace Arenbee.Actors.Default.State;

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(Actor actor)
        : base(actor)
    {
        AddState<NotAttacking>();
        AddState<UnarmedAttack>();
        InitStates(this);
    }

    public class NotAttacking : ActionState
    {
        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override ActionState Update(double delta)
        {
            return CheckForTransitions();
        }

        public override ActionState CheckForTransitions()
        {
            if (StateController.IsBlocked(BlockedState.Attack) || Actor.ContextAreasActive > 0)
                return null;
            if (InputHandler.Attack.IsActionJustPressed)
                return StateMachine.GetState<UnarmedAttack>();
            return null;
        }
    }

    public class UnarmedAttack : ActionState
    {
        public UnarmedAttack()
        {
            AnimationName = "UnarmedAttack";
            BlockedStates = BlockedState.Jumping | BlockedState.Move;
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActionState Update(double delta)
        {
            return CheckForTransitions();
        }

        public override ActionState CheckForTransitions()
        {
            if (StateController.IsBlocked(BlockedState.Attack)
                || Actor.AnimationPlayer.CurrentAnimation != AnimationName)
                return GetState<NotAttacking>();
            if (InputHandler.Attack.IsActionJustPressed)
                return GetState<UnarmedAttack>();
            return null;
        }
    }
}
