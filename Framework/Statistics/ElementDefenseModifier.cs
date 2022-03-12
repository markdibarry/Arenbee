using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class ElementDefenseModifier : Modifier<ElementDefenseModifier>
    {
        public ElementDefenseModifier() { }

        public ElementDefenseModifier(ElementDefenseModifier mod)
        {
            Element = mod.Element;
            Value = mod.Value;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Element Element { get; set; }
    }
}
