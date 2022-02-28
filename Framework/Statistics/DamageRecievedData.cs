using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public class DamageRecievedData
    {
        public string SourceName { get; set; }
        public string RecieverName { get; set; }
        public int TotalDamage { get; set; }
        public Element Element { get; set; }
        public float ElementMultiplier { get; set; }
        public Vector2 SourcePosition { get; set; }
    }
}
