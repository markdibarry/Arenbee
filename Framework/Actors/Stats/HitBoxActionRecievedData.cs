
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Actors.Stats
{
    public class HitBoxActionRecievedData
    {
        public HitBoxActionRecievedData(HitBoxAction hitBoxAction, string recieverName, int totalDamage, float elementMultiplier)
        {
            SourcePosition = hitBoxAction.SourceInfo.SourcePosition;
            SourceName = hitBoxAction.SourceInfo.SourceName;
            RecieverName = recieverName;
            TotalDamage = totalDamage;
            Element = hitBoxAction.Element;
            ElementMultiplier = elementMultiplier;
        }

        public Vector2 SourcePosition { get; set; }
        public string SourceName { get; set; }
        public string RecieverName { get; set; }
        public int TotalDamage { get; set; }
        public Element Element { get; set; }
        public float ElementMultiplier { get; set; }
    }
}
