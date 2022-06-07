using System;

namespace Arenbee.Framework.Statistics
{
    public class ElementDefs : StatDict<ElementDef>
    {
        public ElementDefs()
        {
            StatType = StatType.ElementDef;
        }

        public ElementDefs(ElementDefs defs)
            : this()
        {
            foreach (var pair in defs.StatsDict)
                StatsDict[pair.Key] = new ElementDef(pair.Key, pair.Value);
        }

        protected override ElementDef GetNewStat(int type)
        {
            return new ElementDef(type);
        }

        public ElementDef GetStat(ElementType type)
        {
            return GetStat((int)type);
        }
    }

    public class ElementDef : Stat
    {
        public ElementDef(int type)
            : base(type)
        { }

        public ElementDef(int type, ElementDef elementDef)
            : base(type, elementDef)
        { }

        public const int VeryWeak = 4;
        public const int Weak = 3;
        public const int None = 2;
        public const int Resist = 1;
        public const int Nullify = 0;
        public const int Absorb = -1;

        public ElementType Element
        {
            get => (ElementType)SubType;
            set => SubType = (int)value;
        }

        public override int CalculateStat(bool ignoreHidden = false)
        {
            int result = BaseValue;
            foreach (var mod in Modifiers)
            {
                if (ignoreHidden && mod.IsHidden)
                    continue;
                result += mod.Value - None;
            }

            return Math.Clamp(result + None, Absorb, VeryWeak);
        }

        public static bool Equals(ElementDef a, ElementDef b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            return a.Element == b.Element && a.ModifiedValue == b.ModifiedValue;
        }
    }
}