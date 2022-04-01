using System;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectDefs : StatDict<StatusEffectDef>
    {
        public StatusEffectDefs()
        {
            StatType = StatType.StatusEffectDef;
        }

        public StatusEffectDefs(StatusEffectDefs defs)
            : this()
        {
            foreach (var pair in defs.StatsDict)
                StatsDict[pair.Key] = new StatusEffectDef(pair.Key, pair.Value);
        }

        protected override StatusEffectDef GetNewStat(int type)
        {
            return new StatusEffectDef(type);
        }

        public StatusEffectDef GetStat(StatusEffectType type)
        {
            return GetStat((int)type);
        }
    }

    public class StatusEffectDef : Stat
    {
        public StatusEffectDef(int type)
            : base(type)
        {
            MaxValue = 100;
        }

        public StatusEffectDef(int type, StatusEffectDef statusEffectDef)
            : base(type, statusEffectDef)
        { }

        public StatusEffectType StatusEffectType
        {
            get { return (StatusEffectType)SubType; }
            set { SubType = (int)value; }
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

            ModifiedValue = Math.Min(modifiedValue, MaxValue);
            DisplayValue = Math.Min(displayValue, MaxValue);
        }
    }
}