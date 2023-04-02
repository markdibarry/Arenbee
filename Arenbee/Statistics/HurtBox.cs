using Arenbee.Actors;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public partial class HurtBox : AHurtBox
{
    public override void SetHurtboxRole(int role)
    {
        if (role == (int)ActorRole.Player)
        {
            SetCollisionMaskValue(5, false);
            SetCollisionMaskValue(6, true);
        }
        else if (role == (int)ActorRole.Enemy)
        {
            SetCollisionMaskValue(5, true);
            SetCollisionMaskValue(6, false);
        }
    }
}
