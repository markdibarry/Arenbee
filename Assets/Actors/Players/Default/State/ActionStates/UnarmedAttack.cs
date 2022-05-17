using Arenbee.Assets.Actors.Default.State;
using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.Default.State
{
    public class UnarmedAttack : AttackingState
    {
        public UnarmedAttack()
        {
            AnimationName = "UnarmedAttack";
            BlockedStates = new ActorStateType[]
            {
                ActorStateType.Jump,
                ActorStateType.Move
            };
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override ActorState CheckForTransitions()
        {
            if (Actor.AnimationPlayer.CurrentAnimation != AnimationName)
                return GetState<NotAttacking>();
            if (InputHandler.Attack.IsActionJustPressed)
                return GetState<UnarmedAttack>();
            return null;
        }
    }
}
