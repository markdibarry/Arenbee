using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Statistics;

public partial class StatsData : Resource
{
    public StatsData()
    {
        StatLookup = new();
        Modifiers = new();
    }

    public StatsData(Stats stats)
    {
        StatLookup = stats.StatLookup.Select(x => new Stat(x.Value)).ToGArray();
        Modifiers = stats
            .GetModifiers(ignoreDependentMods: true)
            .Select(x => new Modifier(x))
            .ToGArray();
    }

    public StatsData(StatsData statsData)
    {
        StatLookup = statsData.StatLookup.Select(x => new Stat(x)).ToGArray();
        Modifiers = statsData.Modifiers.Select(x => new Modifier(x)).ToGArray();
    }

    [JsonConstructor]
    public StatsData(Godot.Collections.Array<Stat> statLookup, Godot.Collections.Array<Modifier> modifiers)
    {
        StatLookup = statLookup;
        Modifiers = modifiers;
    }

    public StatsData(IEnumerable<Stat> statLookup, IEnumerable<Modifier> modifiers)
    {
        StatLookup = statLookup.ToGArray();
        Modifiers = modifiers.ToGArray();
    }

    [Export] public Godot.Collections.Array<Stat> StatLookup { get; private set; }
    [Export] public Godot.Collections.Array<Modifier> Modifiers { get; private set; }
}
