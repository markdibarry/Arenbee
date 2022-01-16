using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Dead : State<Player>
    {
        public override void Enter()
        {
            AnimationName = "Dead";
            StateController.ActionStateMachine.TransitionTo(new None());
            StateController.PlayBase(AnimationName);
            Actor.IsWalkDisabled = true;
            Actor.IsAttackDisabled = true;
            Actor.IsJumpDisabled = true;
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
        }
    }
}
