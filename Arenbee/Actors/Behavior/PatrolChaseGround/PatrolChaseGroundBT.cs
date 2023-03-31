using GameCore.Actors;
using GameCore.Input;

namespace Arenbee.Actors.Behavior.PatrolChaseGround;

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
                new TaskChaseTargetOnGround()
            }),
            new TaskPatrol()
        });
    }
}
