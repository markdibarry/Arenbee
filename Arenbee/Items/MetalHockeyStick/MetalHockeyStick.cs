using GameCore.Actors;
using GameCore.Constants;

namespace Arenbee.Items;

public partial class MetalHockeyStick : HockeyStick
{
    public override void Init(ActorBase actor)
    {
        Setup("MetalHockeyStick", WeaponTypeConstants.LongStick, actor, new ActionStateMachine(actor, this));
    }
}
