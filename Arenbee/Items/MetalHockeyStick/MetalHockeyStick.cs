using GameCore;
using GameCore.Actors;

namespace Arenbee.Items;

public partial class MetalHockeyStick : HockeyStick
{
    public override void Init(ActorBase actor)
    {
        Setup("MetalHockeyStick", WeaponTypes.LongStick, actor, new ActionStateMachine(actor, this));
    }
}
