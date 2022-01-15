using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;

namespace Arenbee.Framework.Enums
{
    public enum ModifierEffect
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Percentage

    }

    public class ModifierEffectLookup
    {
        public Dictionary<ModifierEffect, Func<Actor, StatModifier, int>> Lookup { get; set; }

        // public int Add(StatModifier statModifier)
        // {
        //     int thing = Lookup[ModifierEffect.Add](new Ady(), new StatModifier(StatType.Level, true));
        //     return (int)( + statModifier.Value);
        // }

        // public int Subtract(int input)
        // {
        //     return (int)(input - Value);
        // }

        // public int Multiply(int input)
        // {
        //     return (int)(input * Value);
        // }

        // public int Divide(int input)
        // {
        //     if (input == 0 || Value == 0)
        //     {
        //         return 0;
        //     }
        //     return (int)(input / Value);
        // }

        // public int Percentage(int input)
        // {
        //     return (int)(input * (Value / 100));
        // }
    }
}