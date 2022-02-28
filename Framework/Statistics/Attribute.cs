using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public class Attribute
    {
        public Attribute()
        {
            AttributeModifiers = new List<AttributeModifier>();
        }

        public int BaseValue { get; set; }
        [JsonIgnore]
        public int ModifiedValue { get; set; }
        [JsonIgnore]
        public int DisplayValue { get; set; }
        public int MaxValue { get; set; }
        [JsonIgnore]
        public ICollection<AttributeModifier> AttributeModifiers { get; set; }

        public void SetAttribute(int baseValue, int maxValue)
        {
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
