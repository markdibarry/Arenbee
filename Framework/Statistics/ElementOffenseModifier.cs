using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class ElementOffenseModifier : Modifier<ElementOffenseModifier>
    {
        public ElementOffenseModifier() { }

        public ElementOffenseModifier(Element element)
        {
            Element = element;
        }

        public ElementOffenseModifier(ElementOffenseModifier mod)
        {
            Element = mod.Element;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Element Element { get; set; }
    }
}
