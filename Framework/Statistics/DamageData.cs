using Godot;

namespace Arenbee.Framework.Statistics
{
    public class DamageData
    {
        public DamageData()
        {
            Element = Element.None;
            ElementMultiplier = ElementDefense.None;
            TotalDamage = 1;
        }

        public DamageData(ActionInfo actionInfo, int damage, int elementMultiplier)
        {
            Element = actionInfo.Element;
            ElementMultiplier = elementMultiplier;
            SourceName = actionInfo.SourceName;
            SourcePosition = actionInfo.SourcePosition;
            TotalDamage = damage;
        }

        public Element Element { get; set; }
        public int ElementMultiplier { get; set; }
        public string RecieverName { get; set; }
        public string SourceName { get; set; }
        public Vector2 SourcePosition { get; set; }
        public int TotalDamage { get; set; }
    }
}
