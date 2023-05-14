using GameCore.Actors.Behavior;

namespace Arenbee.Actors.Behavior;

public class BTNode : BaseBTNode
{
    protected override ActorBody ActorBody => (ActorBody)ActorBodyInternal;
}
