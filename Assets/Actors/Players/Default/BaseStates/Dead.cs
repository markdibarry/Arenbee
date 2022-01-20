using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Dead : State<Actor>
    {
        public Dead() { AnimationName = "Dead"; }
        public override void Enter()
        {
            StateController.ActionStateMachine.TransitionTo(new None());
            StateMachine.PlayAnimation(AnimationName);
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
