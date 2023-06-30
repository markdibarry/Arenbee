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
    }
}
