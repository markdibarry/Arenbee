using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameCore.Statistics;

public class ElementOffs : IStatSet
{
    public ElementOffs()
    {
        StatType = StatType.ElementOff;
        Modifiers = new List<Modifier>();
    }

    public ElementOffs(ElementOffs elementOffs)
        : this()
    {
        Modifiers = new List<Modifier>(elementOffs.Modifiers);
    }

    [JsonIgnore]
    public ElementType CurrentElement { get { return CalculateStat(); } }
    public StatType StatType { get; set; }
    [JsonIgnore]
    public List<Modifier> Modifiers { get; set; }

    public void AddMod(Modifier mod)
    {
        Modifiers.Add(mod);
    }

    public void RemoveMod(Modifier mod)
    {
        Modifiers.Remove(mod);
    }

    public ElementType CalculateStat()
    {
        int highest = 0;
        var result = ElementType.None;
        foreach (var mod in Modifiers)
        {
            if (mod.Value > highest)
            {
                highest = mod.Value;
                result = (ElementType)mod.SubType;
            }
        }
        return result;
    }
}
