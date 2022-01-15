using System.Collections.Generic;

namespace Arenbee.Framework.Actors.Stats
{
    public class ActorStat
    {
        public ActorStat(StatInfo statInfo)
        {
            StatInfo = statInfo;
            StatModifiers = new List<StatModifier>();
        }

        public int BaseValue { get; set; }
        public int ModifiedValue { get; set; }
        public int DisplayValue { get; set; }
        public int MaxValue { get; set; }
        public StatInfo StatInfo { get; }
        public ICollection<StatModifier> StatModifiers { get; set; }

        public void SetStat(int baseValue, int maxValue)
        {
            BaseValue = baseValue;
            MaxValue = maxValue;
        }

        public void UpdateModifiedValue()
        {
            int modifiedValue = BaseValue;
            int displayValue = modifiedValue;

            foreach (var mod in StatModifiers)
            {
                if (!mod.IsHidden)
                {
                    displayValue = mod.Apply(displayValue);
                }

                modifiedValue = mod.Apply(modifiedValue);
            }

            ModifiedValue = modifiedValue;
            DisplayValue = displayValue;
        }

        // public void ApplyStatMod(StatModifier modifier, int value)
        // {
        //     modifier.
        // }
    }
}
