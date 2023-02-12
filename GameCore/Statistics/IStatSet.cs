using System.Text.Json.Serialization;

namespace GameCore.Statistics;

public enum StatType : byte
{
    None,
    Attribute,
    ElementDef,
    ElementOff,
    StatusEffect,
    StatusEffectDef,
    StatusEffectOff
}

public interface IStatSet
{
    [JsonIgnore]
    StatType StatType { get; set; }
    void AddMod(Modifier mod);
    void RemoveMod(Modifier mod);
}
