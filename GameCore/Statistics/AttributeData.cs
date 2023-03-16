using Arenbee.Statistics;

namespace GameCore.Statistics;

public readonly struct AttributeData
{
    public AttributeData(Stat stat)
        : this((StatType)stat.StatType, stat.Value, stat.MaxValue)
    {
    }

    public AttributeData(StatType statType, int baseValue, int maxValue = 999)
    {
        AttributeType = statType;
        BaseValue = baseValue;
        MaxValue = maxValue;
    }

    public StatType AttributeType { get; }
    public int BaseValue { get; }
    public int MaxValue { get; }
}
