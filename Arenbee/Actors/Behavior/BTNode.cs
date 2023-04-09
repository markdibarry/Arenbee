using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public class BTNode : ABTNode
{
    protected override ActorBody ActorBody => (ActorBody)ActorBodyInternal;
}
