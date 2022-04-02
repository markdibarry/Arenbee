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
            ModEffect effect,
            int value,
            int chance,
            bool isHidden = false)
        {
            StatType = statType;
            SubType = subType;
            Effect = effect;
            Chance = chance;
            Value = value;
            IsHidden = isHidden;
        }

        public Modifier(
            StatType statType,
            int subType,
            ModEffect effect,
            int value,
            bool isHidden = false)
            : this(statType, subType, effect, value, 100, isHidden)
        { }

        public Modifier(
            StatType statType,
            int subType,
            bool isHidden = false)
            : this(statType, subType, ModEffect.None, 0, 100, isHidden)
        { }

        public Modifier(Modifier mod)
        {
            StatType = mod.StatType;
            SubType = mod.SubType;
            Effect = mod.Effect;
            IsHidden = mod.IsHidden;
            Value = mod.Value;
            Chance = mod.Chance;
        }

        public int Chance { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StatType StatType { get; set; }
        public int SubType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ModEffect Effect { get; set; }
        public bool IsHidden { get; set; }
        public int Value { get; }

        public int Apply(int baseValue)
        {
            return s_methods[Effect](baseValue, Value);
        }

        /// <summary>
        /// TODO MAKE BETTER
        /// </summary>
        /// <returns></returns>
        private static readonly Dictionary<ModEffect, Func<int, int, int>> s_methods =
            new()
            {
                { ModEffect.None, None },
                { ModEffect.Add, Add },
                { ModEffect.Subtract, Subtract },
                { ModEffect.Multiply, Multiply },
                { ModEffect.Divide, Divide },
                { ModEffect.Percentage, Percentage }
            };

        private static int None(int baseValue, int modValue)
        {
            return baseValue;
        }

        private static int Add(int baseValue, int modValue)
        {
            return baseValue + modValue;
        }

        private static int Subtract(int baseValue, int modValue)
        {
            return baseValue - modValue;
        }

        public static int Multiply(int baseValue, int modValue)
        {
            return baseValue * modValue;
        }

        public static int Divide(int baseValue, int modValue)
        {
            if (baseValue == 0 || modValue == 0)
                return 0;
            return baseValue / modValue;
        }

        public static int Percentage(int baseValue, int modValue)
        {
            return (int)(baseValue * modValue * 0.01);
        }
    }

    public enum ModEffect
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide,
        Percentage
    }
}