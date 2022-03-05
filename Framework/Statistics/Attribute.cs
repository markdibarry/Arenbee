using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class Attribute
    {
        public Attribute()
        {
            AttributeModifiers = new List<AttributeModifier>();
        }

        [JsonConstructor]
        public Attribute(AttributeType attributeType, int baseValue, int maxValue = 0)
            : this()
        {
            SetAttribute(baseValue, maxValue);
            AttributeType = attributeType;
        }

        public Attribute(Attribute attribute)
            : this()
        {
            AttributeType = attribute.AttributeType;
            foreach (var mod in attribute.AttributeModifiers)
                AttributeModifiers.Add(mod);
            BaseValue = attribute.BaseValue;
            MaxValue = MaxValue;
        }

        [JsonIgnore]
        public string Abbreviation { get { return AttributeType.Get().Abbreviation; } }
        [JsonIgnore]
        public ICollection<AttributeModifier> AttributeModifiers { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AttributeType AttributeType { get; }
        public int BaseValue { get; set; }
        [JsonIgnore]
        public string Description { get { return AttributeType.Get().Description; } }
        [JsonIgnore]
        public int DisplayValue { get; set; }
        public int MaxValue { get; set; }
        [JsonIgnore]
        public int ModifiedValue { get; set; }
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

        public void UpdateModifiedValue()
        {
            int modifiedValue = BaseValue;
            int displayValue = BaseValue;

            foreach (var mod in AttributeModifiers)
            {
                if (!mod.IsHidden)
                    displayValue = mod.Apply(displayValue);
                modifiedValue = mod.Apply(modifiedValue);
            }

            ModifiedValue = modifiedValue;
            DisplayValue = displayValue;
        }
    }
}
