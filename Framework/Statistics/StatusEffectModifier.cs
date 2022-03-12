using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectModifier : Modifier<StatusEffectModifier>
    {
        public StatusEffectModifier() { }

        public StatusEffectModifier(StatusEffectModifier statusEffectModifier)
        {
            StatusEffectType = statusEffectModifier.StatusEffectType;
            Value = statusEffectModifier.Value;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusEffectType StatusEffectType { get; set; }
    }
}
