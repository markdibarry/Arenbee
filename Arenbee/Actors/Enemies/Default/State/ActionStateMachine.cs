using GameCore.Actors;

namespace Arenbee.Actors.Enemies.Default.State;

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(ActorBase actor)
        : base(actor)
    {
        AddState<NotAttacking>();
        InitStates(this);
    }

    public class NotAttacking : ActionState
    {
        public override void Enter() { }

        public override ActionState Update(double delta)
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
