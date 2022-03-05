using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Statistics
{
    public enum Element
    {
        None, Fire, Water, Wind, Earth, Plant, Dark, Light, Healing
    }

    public sealed class ElementData : Enum<Element, ElementData>
    {
        public static readonly ElementData
            None = new ElementData(nameof(None)),
            Fire = new ElementData(nameof(Fire)),
            Water = new ElementData(nameof(Water)),
            Wind = new ElementData(nameof(Wind)),
            Earth = new ElementData(nameof(Earth)),
            Plant = new ElementData(nameof(Plant)),
            Dark = new ElementData(nameof(Dark)),
            Light = new ElementData(nameof(Light)),
            Healing = new ElementData(nameof(Healing), "Heal");

        private ElementData(string name)
            : this(name, name)
        {
        }

        private ElementData(string name, string abbreviation)
        {
            Name = name;
            Abbreviation = abbreviation;
        }

        public string Name { get; }
        public string Abbreviation { get; }
    }

    public static class ElementExtensions
    {
        public static ElementData Get(this Element e) => ElementData.GetData(e);
    }
}
