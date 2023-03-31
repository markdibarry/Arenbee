using Arenbee.Items;

namespace Arenbee.Actors.State.DefaultEnemy;

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(ActorBody actor)
        : base(
            new ActionState[]
            {
                new NotAttacking(actor, null)
            },
            actor, null)
    {
    }

    public class NotAttacking : ActionState
    {
        public NotAttacking(ActorBody actor, HoldItem? holdItem)
            : base(actor, holdItem)
        {
        }

        public override void Enter() { }

        public override void Update(double delta)
        {
        }

        public override void Exit() { }
    }
}
