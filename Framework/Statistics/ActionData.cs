using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public class ActionData
    {
        public ActionData(string sourceName, ActionType actionType)
        {
            ActionType = actionType;
            SourceName = sourceName;
            StatusEffects = new List<Modifier>();
            Value = 1;
        }

        public ActionData(int value, string sourceName, ActionType actionType)
            : this(sourceName, actionType)
        {
            Value = value;
        }

        public ActionType ActionType { get; set; }
        public ElementType ElementDamage { get; set; }
        public string SourceName { get; set; }
        public Vector2 SourcePosition { get; set; }
        public StatusEffectType StatusEffectDamage { get; set; }
        public List<Modifier> StatusEffects { get; set; }
        public int Value { get; set; }
    }
}
