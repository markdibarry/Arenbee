using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class WeakAttack1 : State<Actor>
    {
        public WeakAttack1() { AnimationName = "WeakAttack1"; }
        private bool _canRetrigger;
        private float _retriggerTimer;
        public override void Enter()
        {
            StateMachine.PlayAnimation(AnimationName);
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            if (_retriggerTimer > 0)
                _retriggerTimer -= delta;
            else
                _canRetrigger = true;
        }

        public override void Exit()
        {
            Actor.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
        }

        public void OnAnimationFinished(StringName animationName)
        {
            StateMachine.TransitionTo(new NotAttacking());
            StateController.PlayFallbackAnimation();
        }

        public override void CheckForTransitions()
        {
            if (InputHandler.Attack.IsActionJustPressed && !Actor.IsAttackDisabled && _canRetrigger)
            {
                StateMachine.TransitionTo(new WeakAttack2());
            }
        }
    }
}