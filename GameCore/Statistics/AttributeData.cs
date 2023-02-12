namespace GameCore.Statistics;

public readonly struct AttributeData
{
    public AttributeData(Attribute attribute)
        : this(attribute.AttributeType, attribute.BaseValue, attribute.MaxValue)
    {
    }

    public AttributeData(AttributeType attributeType, int baseValue, int maxValue = 999)
    {
        AttributeType = attributeType;
        BaseValue = baseValue;
        MaxValue = maxValue;
    }

    public AttributeType AttributeType { get; }
    public int BaseValue { get; }
    public int MaxValue { get; }
}
