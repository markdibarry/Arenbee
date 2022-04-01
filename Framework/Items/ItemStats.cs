using System.Collections.Generic;
using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Items
{
    public class ItemStats
    {
        public ItemStats()
        {
            Modifiers = new List<Modifier>();
        }

        public ItemStats(Modifier[] mods)
        {
            Modifiers = mods;
        }

        public IEnumerable<Modifier> Modifiers { get; set; }

        public string GetStatDescription()
        {
            var modParts = new List<string>();
            foreach (var itemMod in Modifiers)
            {
                if (itemMod.IsHidden) continue;
                if (itemMod.StatType == StatType.Attribute)
                {
                    var name = ((AttributeType)itemMod.SubType).Get().Abbreviation;
                    switch (itemMod.Effect)
                    {
                        case ModEffect.Add:
                            modParts.Add($"+{itemMod.Value} {name}");
                            break;
                        case ModEffect.Subtract:
                            modParts.Add($"-{itemMod.Value} {name}");
                            break;
                    }
                }
            }
            return string.Join(", ", modParts);
        }

        public void AddToStats(Stats stats)
        {
            foreach (Modifier mod in Modifiers)
                stats.AddMod(mod);
        }

        public void RemoveFromStats(Stats stats)
        {
            foreach (Modifier mod in Modifiers)
                stats.RemoveMod(mod);
        }
    }
}
