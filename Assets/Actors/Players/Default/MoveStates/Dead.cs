using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Dead : State<Player>
    {
        public override void Enter()
        {
            StateController.ActionStateMachine.TransitionTo(new None());
            StateController.PlayBase("Dead");
        }

        public override void Update()
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
