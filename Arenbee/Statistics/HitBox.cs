using Arenbee.Actors;
using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public partial class HitBox : AHitBox
{
    public HitBox()
    {
        GetDamageRequest = () =>
        {
            return new DamageRequest()
            {
                SourcePosition = GlobalPosition
            };
        };
    }

    public void SetBasicMeleeBox(AActorBody actorBody)
    {
        if (actorBody.Actor == null)
            return;
        GetDamageRequest = () =>
        {
            Stats stats = (Stats)actorBody.Actor.Stats;
            return new DamageRequest()
            {
                SourceName = actorBody.Name,
                ActionType = ActionType.Melee,
                SourcePosition = GlobalPosition,
                ElementType = (ElementType)stats.CalculateStat(StatType.AttackElement),
                StatusChances = stats.GetStatusChances(),
                Value = stats.CalculateStat(StatType.Attack)
            };
        };
    }

    public override void SetHitboxRole(int role)
    {
        if (role == (int)ActorRole.Player)
        {
            SetCollisionLayerValue(6, false);
            SetCollisionLayerValue(5, true);
        }
        else if (role == (int)ActorRole.Enemy)
        {
            SetCollisionLayerValue(6, true);
            SetCollisionLayerValue(5, false);
        }
    }
}
