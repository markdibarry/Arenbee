using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Actors.Stats
{
    public class HitBoxAction
    {
        public HitBoxAction()
        {
            StatusEffects = new List<StatusEffectModifier>();
            SourceInfo = new EventSourceInfo();
            ActionType = ActionType.Environment;
            Element = Element.None;
            Value = 0;
        }

        public HitBoxAction(Node2D node)
        {
            SourceInfo = new EventSourceInfo(node);
        }

        public EventSourceInfo SourceInfo { get; set; }
        public ActionType ActionType { get; set; }
        public IEnumerable<StatusEffectModifier> StatusEffects { get; set; }
        public Element Element { get; set; }
        public int Value { get; set; }
    }
}
