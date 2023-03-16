using Godot;

namespace GameCore.Statistics;

public abstract class AStatTypeDB
{
    public abstract string[] GetTypeNames();
    public abstract string[]? GetValueEnumOptions(int statType);

    public Godot.Collections.Array<Godot.Collections.Dictionary> GetStatPropertyList(int statType)
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> properties = new();
        string[]? valueOptions = GetValueEnumOptions(statType);

        properties.Add(new Godot.Collections.Dictionary()
        {
            { "name", "StatType" },
            { "type", (int)Variant.Type.Int },
            { "usage", (int)PropertyUsageFlags.Default },
            { "hint", (int)PropertyHint.Enum },
            { "hint_string", GetTypeNames().Join(",") }
        });
        if (valueOptions != null)
        {
            properties.Add(new Godot.Collections.Dictionary()
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
            properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", "Value" },
                { "type", (int)Variant.Type.Int },
                { "usage", (int)PropertyUsageFlags.Default }
            });
        }
        return properties;
    }
}
