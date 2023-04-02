using GameCore.Actors;
using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public class PatrolChaseGroundBT : BehaviorTree
{
    public PatrolChaseGroundBT(AActorBody actor) : base(actor) { }

    protected override BTNode SetupTree()
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
