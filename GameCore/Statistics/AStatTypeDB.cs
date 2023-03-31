using Godot;
using Gictionary = Godot.Collections.Dictionary;

namespace GameCore.Statistics;

public abstract class AStatTypeDB
{
    public abstract string[] GetTypeNames();
    public abstract string[]? GetValueEnumOptions(int statType);

    /// <summary>
    /// Gets display properties for editor
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public Godot.Collections.Array<Gictionary> GetStatPropertyList(int statType)
    {
        Godot.Collections.Array<Gictionary> properties = new();
        string[]? valueOptions = GetValueEnumOptions(statType);

        properties.Add(new()
        {
            { "name", "StatType" },
            { "type", (int)Variant.Type.Int },
            { "usage", (int)PropertyUsageFlags.Default },
            { "hint", (int)PropertyHint.Enum },
            { "hint_string", GetTypeNames().Join(",") }
        });

        if (valueOptions != null)
        {
            properties.Add(new()
            {
                { "name", "Value" },
                { "type", (int)Variant.Type.Int },
                { "usage", (int)PropertyUsageFlags.Default },
                { "hint", (int)PropertyHint.Enum },
                { "hint_string", valueOptions.Join(",") }
            });
        }
        else
        {
            properties.Add(new()
            {
                { "name", "Value" },
                { "type", (int)Variant.Type.Int },
                { "usage", (int)PropertyUsageFlags.Default }
            });
        }
        return properties;
    }
}
