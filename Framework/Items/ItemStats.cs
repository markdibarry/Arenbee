using System.Collections.Generic;
using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Items
{
    public class ItemStats
    {
        public ItemStats()
        {
            AttributeModifiers = new AttributeModifier[0];
            ElementDefenseModifiers = new List<ElementDefenseModifier>();
            StatusEffectOffenses = new List<StatusEffectModifier>();
            StatusEffectDefenses = new List<StatusEffectModifier>();
        }

        public IEnumerable<AttributeModifier> AttributeModifiers { get; set; }
        public IEnumerable<ElementDefenseModifier> ElementDefenseModifiers { get; set; }
        public ElementOffenseModifier ElementOffense { get; set; }
        public IEnumerable<StatusEffectModifier> StatusEffectDefenses { get; set; }
        public IEnumerable<StatusEffectModifier> StatusEffectOffenses { get; set; }

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
            foreach (AttributeModifier mod in AttributeModifiers)
                stats.Attributes[mod.AttributeType].Modifiers.Add(mod);

            foreach (ElementDefenseModifier mod in ElementDefenseModifiers)
                stats.AddElementDefenseMod(mod);

            if (ElementOffense != null)
                stats.ElementOffenses.Modifiers.Add(ElementOffense);

            foreach (var mod in StatusEffectDefenses)
                stats.AddStatusEffectDefenseMod(mod);

            foreach (var mod in StatusEffectOffenses)
                stats.AddStatusEffectOffenseMod(mod);
        }

        public void RemoveFromStats(Stats stats)
        {
            foreach (AttributeModifier mod in AttributeModifiers)
                stats.Attributes[mod.AttributeType].Modifiers.Remove(mod);

            foreach (var mod in ElementDefenseModifiers)
                stats.RemoveElementDefenseMod(mod);

            stats.ElementOffenses.Modifiers.Remove(ElementOffense);

            foreach (var mod in StatusEffectDefenses)
                stats.RemoveStatusEffectDefenseMod(mod);

            foreach (var mod in StatusEffectOffenses)
                stats.RemoveStatusEffectOffenseMod(mod);
        }
    }
}
