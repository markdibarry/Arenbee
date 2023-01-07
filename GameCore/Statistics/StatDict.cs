using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameCore.Statistics;

public abstract class StatDict<TStat> : IStatSet
    where TStat : Stat
{
    protected StatDict()
    {
        StatsDict = new Dictionary<int, TStat>();
    }

    [JsonIgnore]
    public StatType StatType { get; set; }
    public IDictionary<int, TStat> StatsDict { get; set; }

    public bool ShouldSerializeStatsDict()
    {
        return StatType == StatType.Attribute;
    }

    public virtual void AddMod(Modifier mod)
    {
        var stat = GetOrNewStat(mod.SubType);
        stat.Modifiers.Add(mod);
    }

    /// <summary>
    /// Gets a Stat via its subtype id
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TStat? GetStat(int key)
    {
        if (!StatsDict.TryGetValue(key, out TStat? stat))
            return null;
        return stat;
    }

    public virtual void RemoveMod(Modifier mod)
    {
        if (!StatsDict.TryGetValue(mod.SubType, out TStat? stat))
            return;
        stat.Modifiers.Remove(mod);
        if (stat.Modifiers.Count == 0)
            RemoveStat(stat);
    }

    public virtual void RemoveStat(TStat stat)
    {
        StatsDict.Remove(stat.SubType);
    }

    protected abstract TStat GetNewStat(int type);

    protected TStat GetOrNewStat(int type)
    {
        if (!StatsDict.TryGetValue(type, out TStat? stat))
        {
            stat = GetNewStat(type);
            StatsDict.Add(type, stat);
        }
        return stat;
    }
}
