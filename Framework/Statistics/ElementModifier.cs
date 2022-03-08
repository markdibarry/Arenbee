namespace Arenbee.Framework.Statistics
{
    public class ElementModifier
    {
        public ElementModifier() { }

        public ElementModifier(ElementModifier mod)
        {
            Element = mod.Element;
            Value = mod.Value;
        }

        public Element Element { get; set; }
        /// <summary>
        /// How much an element should be resisted or strengthened.
        /// Positive values indicate more damage, negative indicate reduced.
        /// </summary>
        /// <value></value>
        public int Value { get; set; }

        public const int VeryWeak = 10;
        public const int Weak = 5;
        public const int None = 0;
        public const int Resist = -5;
        public const int Nullify = -10;
        public const int Absorb = -15;
    }
}
