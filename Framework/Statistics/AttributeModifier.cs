using System;
using System.Collections.Generic;

namespace Arenbee.Framework.Statistics
{
    public class AttributeModifier : Modifier<AttributeModifier>
    {
        public AttributeModifier() { }

        public AttributeModifier(AttributeModifier attributeModifier)
        {
            AttributeType = attributeModifier.AttributeType;
            IsHidden = attributeModifier.IsHidden;
            Effect = attributeModifier.Effect;
            Value = attributeModifier.Value;
        }

        public AttributeType AttributeType { get; set; }
        public ModifierEffect Effect { get; set; }

        public override int Apply(int baseValue)
        {
            return s_methods[Effect](baseValue, Value);
        }

        /// <summary>
        /// TODO MAKE BETTER
        /// </summary>
        /// <returns></returns>
        private static readonly Dictionary<ModifierEffect, Func<int, int, int>> s_methods =
            new Dictionary<ModifierEffect, Func<int, int, int>>()
        {
            { ModifierEffect.Add, Add },
            { ModifierEffect.Subtract, Subtract },
            { ModifierEffect.Multiply, Multiply },
            { ModifierEffect.Divide, Divide },
            { ModifierEffect.Percentage, Percentage }
        };

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
            return (int)(baseValue * (modValue * 0.01));
        }
    }
}
