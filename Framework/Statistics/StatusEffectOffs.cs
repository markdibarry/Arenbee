using System;
using System.Collections.Generic;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectOffs : StatDict<StatusEffectOff>
    {
        public StatusEffectOffs()
        {
            StatType = StatType.StatusEffectOff;
        }

        public StatusEffectOffs(StatusEffectOffs offs)
            : this()
        {
            foreach (var pair in offs.StatsDict)
                StatsDict[pair.Key] = new StatusEffectOff(pair.Key, pair.Value);
        }

        public List<Modifier> GetModifiers()
        {
            var result = new List<Modifier>();
            foreach (var pair in StatsDict)
            {
                if (pair.Value.ModifiedValue != 0)
                    result.Add(new Modifier(StatType.StatusEffect, pair.Key, ModEffect.Add, pair.Value.ModifiedValue, pair.Value.Chance));
            }
            return result;
        }

        protected override StatusEffectOff GetNewStat(int type)
        {
            return new StatusEffectOff(type);
        }
    }

    public class StatusEffectOff : Stat
    {
        public StatusEffectOff(int type)
            : base(type)
        { }

        public StatusEffectOff(int type, StatusEffectOff statusEffectOff)
            : base(type, statusEffectOff)
        { }

        public int Chance { get; set; }
        public StatusEffectType StatusEffectType
        {
            get { return (StatusEffectType)SubType; }
            set { SubType = (int)value; }
        }

        public override void UpdateStat()
        {
            int modifiedValue = BaseValue;
            int displayValue = BaseValue;
            int chance = 0;
            foreach (var mod in Modifiers)
            {
                if (!mod.IsHidden)
                    displayValue = mod.Apply(displayValue);
                modifiedValue = mod.Apply(modifiedValue);
                chance += mod.Chance;
            }

            Chance = Math.Min(chance, 100);
            DisplayValue = displayValue;
            ModifiedValue = modifiedValue;
        }
    }
}