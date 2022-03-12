using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class ElementDefense : Stat<ElementDefenseModifier>
    {
        public ElementDefense()
        {
        }

        public ElementDefense(ElementDefenseModifier elementModifier)
            : this()
        {
            Element = elementModifier.Element;
            Modifiers.Add(elementModifier);
        }

        public ElementDefense(ElementDefense actionElement)
        {
            Element = actionElement.Element;
            Modifiers = new List<ElementDefenseModifier>(actionElement.Modifiers);
            TempModifiers = new List<ElementDefenseModifier>(actionElement.TempModifiers);
        }

        public const int VeryWeak = 4;
        public const int Weak = 3;
        public const int None = 2;
        public const int Resist = 1;
        public const int Nullify = 0;
        public const int Absorb = -1;

        [JsonConverter(typeof(StringEnumConverter))]
        public Element Element { get; set; }

        public override void UpdateStat()
        {
            int modifiedValue = 0;
            int displayValue = 0;
            foreach (var mod in Modifiers)
            {
                if (!mod.IsHidden)
                    displayValue += mod.Value - None;
                modifiedValue += mod.Value - None;
            }

            foreach (var mod in TempModifiers)
            {
                if (!mod.IsHidden)
                    displayValue += mod.Value - None;
                modifiedValue += mod.Value - None;
            }

            ModifiedValue = Math.Clamp(modifiedValue + None, Absorb, VeryWeak);
            DisplayValue = Math.Clamp(displayValue + None, Absorb, VeryWeak);
        }
    }
}
