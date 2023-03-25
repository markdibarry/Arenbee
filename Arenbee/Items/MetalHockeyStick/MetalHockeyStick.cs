using Arenbee.Actors;
using GameCore;

namespace Arenbee.Items;

public partial class MetalHockeyStick : HockeyStick
{
    public override void Init(ActorBody actor)
    {
        Setup(ItemIds.MetalHockeyStick, WeaponTypes.LongStick, actor, new ActionStateMachine(actor, this));
    }
}
