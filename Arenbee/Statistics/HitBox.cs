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
}
