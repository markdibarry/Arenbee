using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public class HitBoxAction
    {
        public HitBoxAction(HitBox hitBox, Node2D source)
        {
            SourceName = source.Name;
            StatusEffects = new List<StatusEffectModifier>();
            Element = Element.None;
            ActionType = ActionType.Environment;
            Value = hitBox.InitialValue;
        }

        public string SourceName { get; set; }
        public IEnumerable<StatusEffectModifier> StatusEffects { get; set; }
        public Element Element { get; set; }
        public ActionType ActionType { get; set; }
        public int Value { get; set; }
    }
}
