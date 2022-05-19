using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class ActionStateMachine : ActionStateMachineBase
    {
        public ActionStateMachine(Actor actor)
            : base(actor)
        {
            AddState<NotAttacking>();
            InitStates(this);
        }

        public class NotAttacking : ActionState
        {
            public override void Enter() { }

            public override ActionState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActionState CheckForTransitions()
            {
                return null;
            }
        }
    }
}