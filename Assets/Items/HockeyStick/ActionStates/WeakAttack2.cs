using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class WeakAttack2 : State<Actor>
    {
        public override void Enter()
        {
            AnimationName = "WeakAttack2";
            StateController.PlayWeaponAttack(AnimationName);
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
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
        }
    }
}