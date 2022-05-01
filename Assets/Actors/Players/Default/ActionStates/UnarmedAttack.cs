using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.ActionStates
{
    public class UnarmedAttack : ActorState
    {
        public UnarmedAttack() { AnimationName = "UnarmedAttack"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
            StateController.BaseStateMachine.TransitionTo(new None());
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit()
        {
            StateController.BaseStateMachine.TransitionTo(new BaseStates.Idle());
        }

        public override ActorState CheckForTransitions()
        {
            if (Actor.AnimationPlayer.CurrentAnimation != AnimationName)
                return new NotAttacking();
            if (!Actor.IsAttackDisabled && InputHandler.Attack.IsActionJustPressed)
                return new UnarmedAttack();
            return null;
        }
    }
}
