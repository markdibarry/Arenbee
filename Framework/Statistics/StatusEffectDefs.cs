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

        public override int CalculateStat(bool ignoreHidden = false)
        {
            int result = BaseValue;

            foreach (var mod in Modifiers)
            {
                if (ignoreHidden && mod.IsHidden)
                    continue;
                result = mod.Apply(result);
            }

            return Math.Min(result, MaxValue);
        }
    }
}