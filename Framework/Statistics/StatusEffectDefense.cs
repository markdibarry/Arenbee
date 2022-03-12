using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectDefense : Stat<StatusEffectModifier>
    {
        public StatusEffectDefense()
        {
            MaxValue = 1;
        }

        public StatusEffectDefense(StatusEffectModifier statusEffectModifier)
            : this()
        {
            StatusEffectType = statusEffectModifier.StatusEffectType;
            Modifiers.Add(statusEffectModifier);
        }

        public StatusEffectDefense(StatusEffectDefense statusEffectDefense)
        {
            MaxValue = 1;
            StatusEffectType = statusEffectDefense.StatusEffectType;
            Modifiers = new List<StatusEffectModifier>(statusEffectDefense.Modifiers);
            TempModifiers = new List<StatusEffectModifier>(statusEffectDefense.TempModifiers);
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
