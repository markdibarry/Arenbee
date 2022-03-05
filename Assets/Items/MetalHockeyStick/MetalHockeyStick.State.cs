using Arenbee.Framework.Items;
using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Items
{
    public partial class MetalHockeyStick : Weapon
    {
        private class NotAttacking : State<Actor>
        {
            public NotAttacking() { IsInitialState = true; }

            public override void Enter()
            {
            }

            public override void Update(float delta)
            {
                CheckForTransitions();
            }

            public override void Exit()
            {
            }

            public override void CheckForTransitions()
            {
                if (InputHandler.Attack.IsActionJustPressed && !Actor.IsAttackDisabled)
                {
                    StateMachine.TransitionTo(new WeakAttack1());
                }
            }
        }

        private class WeakAttack1 : State<Actor>
        {
            public WeakAttack1() { AnimationName = "WeakAttack1"; }
            private bool _canRetrigger;
            private float _retriggerTimer;
            public override void Enter()
            {
                PlayAnimation(AnimationName);
                SubscribeAnimation();
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
                UnsubscribeAnimation();
            }

            protected override void OnAnimationFinished(StringName animationName)
            {
                UnsubscribeAnimation();
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

        private class WeakAttack2 : State<Actor>
        {
            public WeakAttack2() { AnimationName = "WeakAttack2"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
                SubscribeAnimation();
            }

            public override void Update(float delta)
            {
                CheckForTransitions();
            }

            public override void Exit()
            {
                UnsubscribeAnimation();
            }

            protected override void OnAnimationFinished(StringName animationName)
            {
                UnsubscribeAnimation();
                StateMachine.TransitionTo(new NotAttacking());
                StateController.PlayFallbackAnimation();
            }

            public override void CheckForTransitions()
            {
            }
        }
    }
}
