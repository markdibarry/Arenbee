using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.Statistics
{
    public class ActionData
    {
        public ActionData()
        {
            StatusEffects = new List<Modifier>();
            Value = 1;
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
