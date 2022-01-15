using Arenbee.Framework;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class WeakAttack1 : State<Actor>
    {
        private bool _canRetrigger;
        private Timer _retriggerTimer;
        public override void Enter()
        {
            StateController.PlayWeaponAttack("WeakAttack1");
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
            _retriggerTimer = Actor.CreateOneShotTimer(0.1f);
            _retriggerTimer.Timeout += OnRepeatAttackTimerTimeout;
        }

        public override void Update()
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
            StateController.PlayLastBaseAnimation();
        }

        public override void CheckForTransitions()
        {
            if (Input.IsActionJustPressed(ActionConstants.Attack) && _canRetrigger)
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