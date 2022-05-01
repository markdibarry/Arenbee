using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class Attributes : StatDict<Attribute>
    {
        public Attributes()
        {
            StatType = StatType.Attribute;
            foreach (int type in Enum.GetValues(typeof(AttributeType)))
                StatsDict[type] = new Attribute(type);
        }

        public Attributes(Attributes attributes)
        {
            StatType = StatType.Attribute;
            foreach (var pair in attributes.StatsDict)
                StatsDict[pair.Key] = new Attribute(pair.Key, pair.Value);
        }

        /// <summary>
        /// Applies a set of Stats to the current ones
        /// </summary>
        /// <param name="stats"></param>
        public void ApplyStats(IDictionary<int, Attribute> stats)
        {
            foreach (var pair in stats)
            {
                var stat = GetOrNewStat(pair.Key);
                stat.BaseValue = pair.Value.BaseValue;
                stat.MaxValue = pair.Value.MaxValue;
            }
        }

        public Attribute GetStat(AttributeType type)
        {
            return GetStat((int)type);
        }

        protected override Attribute GetNewStat(int type)
        {
            return new Attribute(type);
        }

        public override void RemoveMod(Modifier mod)
        {
            if (StatsDict.TryGetValue(mod.SubType, out var stat))
                stat.Modifiers.Remove(mod);
        }
    }

    public class Attribute : Stat
    {
        public Attribute(int type)
            : base(type)
        {
            switch ((AttributeType)type)
            {
                case AttributeType.Level:
                    BaseValue = 1;
                    MaxValue = 99;
                    break;
                case AttributeType.HP:
                case AttributeType.MaxHP:
                    BaseValue = 1;
                    MaxValue = 999;
                    break;
                default:
                    BaseValue = 0;
                    MaxValue = 999;
                    break;
            }
        }

        [JsonConstructor]
        public Attribute(AttributeType attributeType, int baseValue, int maxValue)
            : base((int)attributeType, baseValue, maxValue)
        { }

        public Attribute(int type, Attribute attribute)
            : base(type, attribute)
        { }

        [JsonConverter(typeof(StringEnumConverter))]
        public AttributeType AttributeType
        {
            get { return (AttributeType)SubType; }
            set { SubType = (int)value; }
        }

        public void SetAttribute(int baseValue, int maxValue)
        {
            BaseValue = baseValue;
            MaxValue = maxValue;
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