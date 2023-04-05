using Arenbee.Input;
using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public static class StatusEffectMethods
{
    public static void BurnTick(IStatusEffect statusEffect)
    {
        AStats stats = statusEffect.Stats;
        IDamageable statsOwner = stats.StatsOwner;
        if (statsOwner is AActor actor && actor.ActorBody != null)
            ((ActorInputHandler)actor.ActorBody.InputHandler).Jump.IsActionJustPressed = true;
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
        AStats stats = statusEffect.Stats;
        DamageRequest damageRequest = new()
        {
            Value = (int)(stats.CalculateStat((int)StatType.MaxHP) * 0.05),
            SourceName = statusEffect.EffectData.Name,
            ActionType = ActionType.Status
        };
        stats.ReceiveDamageRequest(damageRequest);
    }
}
