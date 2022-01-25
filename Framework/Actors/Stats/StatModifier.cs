using System;
using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors.Stats
{
    public class StatModifier
    {
        public StatType StatType { get; set; }
        public bool IsHidden { get; } = false;
        public ModifierEffect Effect { get; set; }
        public float Value { get; set; }

        public int Apply(Actor actor, int baseValue)
        {
            return s_methods[Effect](actor, baseValue, Value);
        }

        private static readonly Dictionary<ModifierEffect, Func<Actor, int, float, int>> s_methods =
            new Dictionary<ModifierEffect, Func<Actor, int, float, int>>()
        {
            { ModifierEffect.Add, Add },
            { ModifierEffect.Subtract, Subtract },
            { ModifierEffect.Multiply, Multiply },
            { ModifierEffect.Divide, Divide },
            { ModifierEffect.Percentage, Percentage }
        };

        private static int Add(Actor actor, int baseValue, float modValue)
        {
            return (int)(baseValue + modValue);
        }

        private static int Subtract(Actor actor, int baseValue, float modValue)
        {
            return (int)(baseValue - modValue);
        }

        public static int Multiply(Actor actor, int baseValue, float modValue)
        {
            return (int)(baseValue * modValue);
        }

        public static int Divide(Actor actor, int baseValue, float modValue)
        {
            if (baseValue == 0 || modValue == 0)
            {
                return 0;
            }
            return (int)(baseValue / modValue);
        }

        public static int Percentage(Actor actor, int baseValue, float modValue)
        {
            return (int)(baseValue * (modValue / 100));
        }
    }
}
