using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public abstract class BehaviorTree : BaseBehaviorTree
{
    protected BehaviorTree(ActorBody actorBody)
        : base(actorBody)
    {
    }
}
