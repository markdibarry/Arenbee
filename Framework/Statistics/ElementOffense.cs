using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public class ElementOffense : Stat<ElementOffenseModifier>
    {
        public ElementOffense()
        {
            Modifiers.Add(new ElementOffenseModifier(Element.None));
        }

        public ElementOffense(ElementOffenseModifier elementModifier)
            : this()
        {
            Modifiers.Add(elementModifier);
        }

        public ElementOffense(ElementOffense actionElement)
        {
            Modifiers = new List<ElementOffenseModifier>(actionElement.Modifiers);
            TempModifiers = new List<ElementOffenseModifier>(actionElement.TempModifiers);
        }

        [JsonIgnore]
        public Element CurrentElement { get; set; }

        public override void UpdateStat()
        {
            if (TempModifiers.Count > 0)
                CurrentElement = TempModifiers.Last().Element;
            else if (Modifiers.Count > 0)
                CurrentElement = Modifiers.Last().Element;
            else
                CurrentElement = Element.None;
        }
    }
}
