using System;
using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Statistics
{
    public class AttributeModifier
    {
        public AttributeType AttributeType { get; set; }
        public bool IsHidden { get; } = false;
        public ModifierEffect Effect { get; set; }
        public float Value { get; set; }

        public int Apply(int baseValue)
        {
            return s_methods[Effect](baseValue, Value);
        }

        /// <summary>
        /// TODO MAKE BETTER
        /// </summary>
        /// <returns></returns>
        private static readonly Dictionary<ModifierEffect, Func<int, float, int>> s_methods =
            new Dictionary<ModifierEffect, Func<int, float, int>>()
        {
            { ModifierEffect.Add, Add },
            { ModifierEffect.Subtract, Subtract },
            { ModifierEffect.Multiply, Multiply },
            { ModifierEffect.Divide, Divide },
            { ModifierEffect.Percentage, Percentage }
        };

        private static int Add(int baseValue, float modValue)
        {
            return (int)(baseValue + modValue);
        }

        private static int Subtract(int baseValue, float modValue)
        {
            return (int)(baseValue - modValue);
        }

        public static int Multiply(int baseValue, float modValue)
        {
            return (int)(baseValue * modValue);
        }

        public static int Divide(int baseValue, float modValue)
        {
            if (baseValue == 0 || modValue == 0)
            {
                return 0;
            }
            return (int)(baseValue / modValue);
        }

        public static int Percentage(int baseValue, float modValue)
        {
            return (int)(baseValue * (modValue / 100));
        }
    }
}
