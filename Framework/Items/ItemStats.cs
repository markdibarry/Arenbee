using System.Collections.Generic;
using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Items
{
    public class ItemStats
    {
        public ItemStats()
        {
            AttributeModifiers = new AttributeModifier[0];
            ActionStatusEffects = new StatusEffectModifier[0];
            DefenseStatusEffects = new StatusEffectModifier[0];
            DefenseElementModifiers = new ElementModifier[0];
        }

        public Element ActionElement { get; set; }
        public IEnumerable<StatusEffectModifier> ActionStatusEffects { get; set; }
        public IEnumerable<AttributeModifier> AttributeModifiers { get; set; }
        public IEnumerable<StatusEffectModifier> DefenseStatusEffects { get; set; }
        public IEnumerable<ElementModifier> DefenseElementModifiers { get; set; }

        public string GetStatDescription()
        {
            var modParts = new List<string>();
            foreach (var mod in AttributeModifiers)
            {
                if (mod.IsHidden) continue;
                var name = mod.AttributeType.Get().Abbreviation;
                switch (mod.Effect)
                {
                    case ModifierEffect.Add:
                        modParts.Add($"+{mod.Value} {name}");
                        break;
                    case ModifierEffect.Subtract:
                        modParts.Add($"-{mod.Value} {name}");
                        break;
                }
            }
            return string.Join(", ", modParts);
        }

        public void AddToStats(Stats stats)
        {
            foreach (ElementModifier mod in DefenseElementModifiers)
                stats.DefenseElementModifiers.Add(mod);

            foreach (StatusEffectModifier mod in ActionStatusEffects)
                stats.ActionStatusEffects.Add(mod);

            foreach (StatusEffectModifier mod in DefenseStatusEffects)
                stats.DefenseStatusEffects.Add(mod);

            foreach (AttributeModifier mod in AttributeModifiers)
                stats.GetAttribute(mod.AttributeType).AttributeModifiers.Add(mod);
        }

        public void RemoveFromStats(Stats stats)
        {
            foreach (ElementModifier mod in DefenseElementModifiers)
                stats.DefenseElementModifiers.Remove(mod);

            foreach (StatusEffectModifier mod in ActionStatusEffects)
                stats.ActionStatusEffects.Remove(mod);

            foreach (StatusEffectModifier mod in DefenseStatusEffects)
                stats.DefenseStatusEffects.Remove(mod);

            foreach (AttributeModifier mod in AttributeModifiers)
                stats.GetAttribute(mod.AttributeType).AttributeModifiers.Remove(mod);
        }
    }
}
