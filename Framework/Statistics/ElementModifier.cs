using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Statistics
{
    public class ElementModifier
    {
        public Element Element { get; set; }
        /// <summary>
        /// How much an element should be resisted or strengthened.
        /// Positive values indicate more damage, negative indicate reduced.
        /// </summary>
        /// <value></value>
        public float Value { get; set; }

        public const float VeryWeak = 1f;
        public const float Weak = 0.5f;
        public const float Resist = -0.5f;
        public const float Nullify = -1f;
        public const float Absorb = -1.5f;
    }
}
