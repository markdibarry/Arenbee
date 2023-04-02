using GameCore.Actors;
using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public class PatrolChaseAirBT : BehaviorTree
{
    public PatrolChaseAirBT(AActorBody actor) : base(actor) { }

    protected override BTNode SetupTree()
    {
        return new Selector(new()
        {
            new Sequence(new()
            {
                new CheckTargetInArea(),
                new TaskChaseTarget()
            }),
            new Sequence(new()
            {
                new Inverter(new()
                {
                    new CheckIsHome()
                }),
                new TaskMoveHome()
            })
        });
    }
}
