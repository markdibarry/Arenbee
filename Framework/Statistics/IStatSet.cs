using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public enum StatType
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
}