using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public class PatrolChaseGroundBT : BehaviorTree
{
    public PatrolChaseGroundBT(ActorBody actor) : base(actor) { }

    protected override ABTNode SetupTree()
    {
        return new Selector(new()
        {
            new Sequence(new()
            {
                new CheckTargetInRayCastRange(),
                new TaskChaseTarget()
            }),
            new TaskPatrol()
        });
    }
}
