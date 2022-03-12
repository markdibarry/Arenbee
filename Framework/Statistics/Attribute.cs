using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class Attribute : Stat<AttributeModifier>
    {
        [JsonConstructor]
        public Attribute(AttributeType attributeType, int baseValue, int maxValue = 0)
        {
            SetAttribute(baseValue, maxValue);
            AttributeType = attributeType;
        }

        public Attribute(Attribute attribute)
        {
            AttributeType = attribute.AttributeType;
            Modifiers = new List<AttributeModifier>(attribute.Modifiers);
            TempModifiers = new List<AttributeModifier>(attribute.TempModifiers);
            BaseValue = attribute.BaseValue;
            MaxValue = attribute.MaxValue;
        }

        [JsonIgnore]
        public string Abbreviation { get { return AttributeType.Get().Abbreviation; } }
        [JsonConverter(typeof(StringEnumConverter))]
        public AttributeType AttributeType { get; }
        [JsonIgnore]
        public string Description { get { return AttributeType.Get().Description; } }
        [JsonIgnore]
        public string Name { get { return AttributeType.Get().Name; } }

        public void SetAttribute(int baseValue, int maxValue)
        {
            if (maxValue == 0)
            {
                switch (AttributeType)
                {
                    case AttributeType.Level:
                    case AttributeType.Speed:
                        maxValue = 99;
                        break;
                    default:
                        maxValue = 999;
                        break;
                }
            }
            BaseValue = baseValue;
            MaxValue = maxValue;
        }

        public override void UpdateStat()
        {
            int modifiedValue = BaseValue;
            int displayValue = BaseValue;

            foreach (var mod in Modifiers)
            {
                if (!mod.IsHidden)
                    displayValue = mod.Apply(displayValue);
                modifiedValue = mod.Apply(modifiedValue);
            }
            foreach (var mod in TempModifiers)
            {
                if (!mod.IsHidden)
                    displayValue = mod.Apply(displayValue);
                modifiedValue = mod.Apply(modifiedValue);
            }

            ModifiedValue = Math.Min(modifiedValue, MaxValue);
            DisplayValue = Math.Min(displayValue, MaxValue);
        }
    }
}
