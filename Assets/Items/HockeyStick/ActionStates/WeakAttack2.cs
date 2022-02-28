using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class WeakAttack2 : State<Actor>
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