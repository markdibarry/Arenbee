namespace Arenbee.Framework.Statistics
{
    public interface IModifier
    {
        public StatType StatType { get; set; }
        public int SubType { get; set; }
        public ModEffect Effect { get; set; }
        public bool IsHidden { get; set; }
        public int Value { get; }
        public int Chance { get; }
        public int Apply(int baseValue);
    }
}