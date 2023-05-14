using Arenbee.Actors;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public static class StatusEffectMethods
{
    public static void BurnTick(IStatusEffect statusEffect)
    {
        BaseStats stats = statusEffect.Stats;
        if (stats.StatsOwner is Actor actor && actor.ActorBody != null)
            actor.ActorBody.InputHandler.Jump.IsActionJustPressed = true;
        DamageRequest damageRequest = new()
        {
            Value = (int)(stats.CalculateStat((int)StatType.MaxHP) * 0.05),
            SourceName = statusEffect.EffectData.Name,
            ActionType = ActionType.Status
        };
        stats.ReceiveDamageRequest(damageRequest);
    }

    public static void PoisonTick(IStatusEffect statusEffect)
    {
        BaseStats stats = statusEffect.Stats;
        DamageRequest damageRequest = new()
        {
            Value = (int)(stats.CalculateStat((int)StatType.MaxHP) * 0.05),
            SourceName = statusEffect.EffectData.Name,
            ActionType = ActionType.Status
        };
        stats.ReceiveDamageRequest(damageRequest);
    }
}
