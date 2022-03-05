using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public class HitBoxAction
    {
        public HitBoxAction(HitBox hitBox, Node2D source)
        {
            ActionType = ActionType.Environment;
            Element = Element.None;
            SourceName = source.Name;
            StatusEffects = new List<StatusEffectModifier>();
            Value = hitBox.InitialValue;
        }

        public ActionType ActionType { get; set; }
        public Element Element { get; set; }
        public string SourceName { get; set; }
        public Vector2 SourcePosition { get; set; }
        public IEnumerable<StatusEffectModifier> StatusEffects { get; set; }
        public int Value { get; set; }
    }
}
