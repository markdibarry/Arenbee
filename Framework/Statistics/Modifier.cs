using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class Modifier
    {
        [JsonConstructor]
        public Modifier(
            StatType statType,
            int subType,
            ModOperator effect,
            int value,
            int chance,
            bool isHidden = false)
        {
            StatType = statType;
            SubType = subType;
            Operator = effect;
            Chance = chance;
            Value = value;
            IsHidden = isHidden;
        }

        public Modifier(
            StatType statType,
            int subType,
            ModOperator effect,
            int value,
            bool isHidden = false)
            : this(statType, subType, effect, value, 100, isHidden)
        { }

        public Modifier(
            StatType statType,
            int subType,
            bool isHidden = false)
            : this(statType, subType, ModOperator.None, 0, 100, isHidden)
        { }

        public Modifier(Modifier mod)
        {
            StatType = mod.StatType;
            SubType = mod.SubType;
            Operator = mod.Operator;
            IsHidden = mod.IsHidden;
            Value = mod.Value;
            Chance = mod.Chance;
        }

        public int Chance { get; set; }
        public bool IsHidden { get; set; }
        public ModOperator Operator { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StatType StatType { get; set; }
        public int SubType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public int Value { get; }

        public int Apply(int baseValue)
        {
            return s_methods[Operator](baseValue, Value);
        }

        /// <summary>
        /// TODO MAKE BETTER
        /// </summary>
        /// <returns></returns>
        private static readonly Dictionary<ModOperator, Func<int, int, int>> s_methods =
            new()
            {
                { ModOperator.None, None },
                { ModOperator.Add, Add },
                { ModOperator.Multiply, Multiply }
            };

        private static int None(int baseValue, int modValue)
        {
            return baseValue;
        }

        private static int Add(int baseValue, int modValue)
        {
            return baseValue + modValue;
        }

        public static int Multiply(int baseValue, int modValue)
        {
            return (int)(baseValue + (baseValue * modValue * 0.01));
        }
    }

    public enum ModOperator
    {
        None,
        Add,
        Multiply
    }
}