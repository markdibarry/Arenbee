using System.Collections.Generic;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;
using System.Text;
using System.Linq;

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

        public IEnumerable<AttributeModifier> AttributeModifiers { get; set; }
        public IEnumerable<StatusEffectModifier> ActionStatusEffects { get; set; }
        public IEnumerable<StatusEffectModifier> DefenseStatusEffects { get; set; }
        public Element ActionElement { get; set; }
        public IEnumerable<ElementModifier> DefenseElementModifiers { get; set; }

        public string GetStatDescription()
        {
            var modParts = new List<string>();
            foreach (var mod in AttributeModifiers)
            {
                if (mod.IsHidden) continue;
                switch (mod.Effect)
                {
                    case ModifierEffect.Add:
                        modParts.Add($"+{mod.Value} {mod.AttributeType}");
                        break;
                    case ModifierEffect.Subtract:
                        modParts.Add($"-{mod.Value} {mod.AttributeType}");
                        break;
                }
            }
            return string.Join(", ", modParts);
        }
    }
}
