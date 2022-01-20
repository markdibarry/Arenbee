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
            StateMachine.PlayAnimation(AnimationName);
            StateController.BaseStateMachine.TransitionTo(new None());
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            StateController.BaseStateMachine.TransitionTo(new BaseStates.Idle());
        }

        public void OnAnimationFinished(StringName animationName)
        {
            StateMachine.TransitionTo(new NotAttacking());
            StateController.PlayFallbackAnimation();
        }

        public override void CheckForTransitions()
        {
            if (InputHandler.Attack.IsActionJustPressed && !Actor.IsAttackDisabled)
            {
                StateMachine.TransitionTo(new UnarmedAttack());
            }
        }
    }
}
