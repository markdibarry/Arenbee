namespace Arenbee.Statistics;

public readonly struct StatusChance
{
    public StatusChance(StatusEffectType statusEffectType, int chance)
    {
        StatusEffectType = statusEffectType;
        Chance = chance;
    }

    public StatusEffectType StatusEffectType { get; }
    public int Chance { get; }
}
