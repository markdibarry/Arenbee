using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class WeakAttack2 : State<Actor>
    {
        public override void Enter()
        {
            StateController.PlayWeaponAttack("WeakAttack2");
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
        }

        public override void Update()
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
            StateController.PlayLastBaseAnimation();
        }

        public override void CheckForTransitions()
        {
        }
    }
}