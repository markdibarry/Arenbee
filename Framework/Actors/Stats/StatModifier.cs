using System;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors.Stats
{
    public class StatModifier
    {
        public StatModifier(StatType statType, bool isHidden = false)
        {
            StatType = statType;
            IsHidden = isHidden;
        }

        public StatType StatType { get; set; }
        public bool IsHidden { get; }
        public Func<int, int> Apply { get; set; }
        public ModifierEffect ModifierEffect { get; set; }
        public float Value { get; set; }


    }
}