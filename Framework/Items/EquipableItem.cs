using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class EquipableItem : Item
    {
        public EquipmentType EquipmentType { get; set; }
        public IEnumerable<StatModifier> StatModifiers { get; set; }
        public IEnumerable<StatusEffectModifier> ActionStatusEffects { get; set; }
        public IEnumerable<StatusEffectModifier> DefenseStatusEffects { get; set; }
        public Element ActionElement { get; set; }
        public IEnumerable<ElementModifier> DefenseElementModifiers { get; set; }
    }
}
