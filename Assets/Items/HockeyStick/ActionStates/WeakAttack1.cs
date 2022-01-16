using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class WeakAttack1 : State<Actor>
    {
        private bool _canRetrigger;
        private Timer _retriggerTimer;
        public override void Enter()
        {
            AnimationName = "WeakAttack1";
            StateController.PlayWeaponAttack(AnimationName);
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
            _retriggerTimer = Actor.CreateOneShotTimer(0.1f);
            _retriggerTimer.Timeout += OnRepeatAttackTimerTimeout;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            Actor.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
            if (Object.IsInstanceValid(_retriggerTimer))
                _retriggerTimer.QueueFree();
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

        public void OnRepeatAttackTimerTimeout()
        {
            _canRetrigger = true;
        }
    }
}