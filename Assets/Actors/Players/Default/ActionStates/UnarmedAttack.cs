using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Actors.Players.ActionStates
{
    public class UnarmedAttack : State<Actor>
    {
        public UnarmedAttack() { AnimationName = "UnarmedAttack"; }
        public override void Enter()
        {
            PlayAnimation(AnimationName);
            SubscribeAnimation();
            StateController.BaseStateMachine.TransitionTo(new None());
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            UnsubscribeAnimation();
            StateController.BaseStateMachine.TransitionTo(new BaseStates.Idle());
        }

        protected override void OnAnimationFinished(StringName animationName)
        {
            UnsubscribeAnimation();
            StateMachine.TransitionTo(new NotAttacking());
            StateController.PlayFallbackAnimation();
        }

        public override void CheckForTransitions()
        {
            if (InputHandler.Attack.IsActionJustPressed && !Actor.IsAttackDisabled)
                StateMachine.TransitionTo(new UnarmedAttack());
        }
    }
}
