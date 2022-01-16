using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Players.ActionStates
{
    public class UnarmedAttack : State<Player>
    {
        public override void Enter()
        {
            StateController.PlayWeaponAttack("UnarmedAttack");
            StateController.BaseStateMachine.TransitionTo(new None());
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            StateController.BaseStateMachine.TransitionTo(new MoveStates.Idle());
        }

        public void OnAnimationFinished(StringName animationName)
        {
            StateMachine.TransitionTo(new NotAttacking());
            StateController.PlayFallbackAnimation();
        }

        public override void CheckForTransitions()
        {
            if (StateMachine.IsActionPressed(Actor.InputHandler.Attack))
            {
                StateMachine.TransitionTo(new UnarmedAttack());
            }
        }
    }
}
