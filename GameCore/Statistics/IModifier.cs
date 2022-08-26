namespace GameCore.Statistics
{
    public interface IModifier
    {
        public StatType StatType { get; set; }
        public int SubType { get; set; }
        public ModOperator Operator { get; set; }
        public bool IsHidden { get; set; }
        public int Value { get; }
        public int Chance { get; }
        public int Apply(int baseValue);
    }
}