using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arenbee.Framework.Actors.Stats
{
    public class ActorStat
    {
        public ActorStat()
        {
            StatModifiers = new List<StatModifier>();
        }

        public int BaseValue { get; set; }
        [JsonIgnore]
        public int ModifiedValue { get; set; }
        [JsonIgnore]
        public int DisplayValue { get; set; }
        public int MaxValue { get; set; }
        [JsonIgnore]
        public ICollection<StatModifier> StatModifiers { get; set; }

        public void SetStat(int baseValue, int maxValue)
        {
            BaseValue = baseValue;
            MaxValue = maxValue;
        }

        public void UpdateModifiedValue(Actor actor)
        {
            int modifiedValue = BaseValue;
            int displayValue = BaseValue;

            foreach (var mod in StatModifiers)
            {
                if (!mod.IsHidden)
                {
                    displayValue = mod.Apply(actor, displayValue);
                }

                modifiedValue = mod.Apply(actor, modifiedValue);
            }

            ModifiedValue = modifiedValue;
            DisplayValue = displayValue;
        }
    }
}
