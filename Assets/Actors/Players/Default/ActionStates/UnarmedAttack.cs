using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;
using Arenbee.Framework.Constants;

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

        public override void Update()
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
            StateController.PlayLastBaseAnimation();
        }

        public override void CheckForTransitions()
        {
            if (Input.IsActionJustPressed(ActionConstants.Attack))
            {
                StateMachine.TransitionTo(new UnarmedAttack());
            }
        }
    }
}
