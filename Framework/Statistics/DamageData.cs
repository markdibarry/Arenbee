using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public class DamageData
    {
        public DamageData(HitBoxAction hitBoxAction, int damage, float elementMultiplier)
        {
            Element = hitBoxAction.Element;
            ElementMultiplier = elementMultiplier;
            SourceName = hitBoxAction.SourceName;
            SourcePosition = hitBoxAction.SourcePosition;
            TotalDamage = damage;
        }

        public Element Element { get; set; }
        public float ElementMultiplier { get; set; }
        public string RecieverName { get; set; }
        public string SourceName { get; set; }
        public Vector2 SourcePosition { get; set; }
        public int TotalDamage { get; set; }
    }
}
