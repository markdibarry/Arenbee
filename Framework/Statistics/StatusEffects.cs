using System.Collections.Generic;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffects : Stat<StatusEffect>
    {
        public StatusEffects()
        {
            MaxValue = 1;
        }

        public StatusEffects(StatusEffect statusEffect)
            : this()
        {
            Modifiers.Add(statusEffect);
        }

        public StatusEffects(StatusEffects statusEffects)
        {
            Modifiers = new List<StatusEffect>(statusEffects.Modifiers);
            TempModifiers = new List<StatusEffect>(statusEffects.TempModifiers);
        }
        public override void UpdateStat()
        {
        }
    }
}