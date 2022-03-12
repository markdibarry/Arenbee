using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectOffense : Stat<StatusEffectModifier>
    {
        public StatusEffectOffense()
        {
            MaxValue = 1;
        }

        public StatusEffectOffense(StatusEffectModifier statusEffectModifier)
            : this()
        {
            StatusEffectType = statusEffectModifier.StatusEffectType;
            Modifiers.Add(statusEffectModifier);
        }

        public StatusEffectOffense(StatusEffectOffense actionStatusEffect)
        {
            MaxValue = 1;
            StatusEffectType = actionStatusEffect.StatusEffectType;
            Modifiers = new List<StatusEffectModifier>(actionStatusEffect.Modifiers);
            TempModifiers = new List<StatusEffectModifier>(actionStatusEffect.TempModifiers);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusEffectType StatusEffectType { get; set; }

        public override void UpdateStat()
        {
            int modifiedValue = 0;
            int displayValue = 0;
            foreach (var mod in Modifiers)
            {
                if (!mod.IsHidden)
                    displayValue += mod.Value;
                modifiedValue += mod.Value;
            }

            foreach (var mod in TempModifiers)
            {
                if (!mod.IsHidden)
                    displayValue += mod.Value;
                modifiedValue += mod.Value;
            }

            ModifiedValue = Math.Min(modifiedValue, MaxValue);
            DisplayValue = Math.Min(displayValue, MaxValue);
        }
    }
}
