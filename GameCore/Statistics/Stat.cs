using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameCore.Statistics;

public abstract class Stat
{
    /// <summary>
    /// Creates a new instance of Stat
    /// </summary>
    protected Stat(int type)
    {
        SubType = type;
        BaseValue = 0;
        MaxValue = 999;
        Modifiers = new();
    }

    protected Stat(int type, int baseValue, int maxValue)
        : this(type)
    {
        BaseValue = baseValue;
        MaxValue = maxValue;
    }

    /// <summary>
    /// Creates a clone of a Stat
    /// </summary>
    /// <param name="valueStat"></param>
    protected Stat(int type, Stat valueStat)
    {
        SubType = type;
        BaseValue = valueStat.BaseValue;
        MaxValue = valueStat.MaxValue;
        Modifiers = new(valueStat.Modifiers);
    }

    public int BaseValue { get; set; }
    public int MaxValue { get; set; }
    [JsonIgnore]
    public int DisplayValue => CalculateStat(true);
    [JsonIgnore]
    public int ModifiedValue => CalculateStat();
    [JsonIgnore]
    public List<Modifier> Modifiers { get; set; }
    [JsonIgnore]
    public int SubType { get; protected set; }

    public abstract int CalculateStat(bool ignoreHidden = false);
}
