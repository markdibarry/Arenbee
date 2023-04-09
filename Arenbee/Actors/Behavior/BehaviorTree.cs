using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public abstract class BehaviorTree : ABehaviorTree
{
    protected BehaviorTree(ActorBody actorBody)
        : base(actorBody)
    {
    }
}
