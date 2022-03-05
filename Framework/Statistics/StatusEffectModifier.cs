using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectModifier
    {
        public StatusEffectModifier() { }

        public StatusEffectModifier(StatusEffectModifier statusEffectModifier)
        {
            StatusEffect = statusEffectModifier.StatusEffect;
            Value = statusEffectModifier.Value;
        }

        public StatusEffect StatusEffect { get; set; }
        public float Value { get; set; }
    }
}
