using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public enum Element
    {
        None, Fire, Water, Wind, Earth, Plant, Dark, Light, Healing
    }

    public sealed class ElementData : Enum<Element, ElementData>
    {
        public static readonly ElementData
            None = new ElementData(nameof(None), Colors.Gray),
            Fire = new ElementData(nameof(Fire), new Color(0.78f, 0.12f, 0.06f)),
            Water = new ElementData(nameof(Water), new Color(0.2f, 0.25f, 0.78f)),
            Wind = new ElementData(nameof(Wind), new Color(0.27f, 0.78f, 0.59f)),
            Earth = new ElementData(nameof(Earth), new Color(0.71f, 0.51f, 0.2f)),
            Plant = new ElementData(nameof(Plant), new Color(0.33f, 0.73f, 0)),
            Dark = new ElementData(nameof(Dark), new Color(0.33f, 0.33f, 0.39f)),
            Light = new ElementData(nameof(Light), new Color(0.69f, 0.67f, 0.78f)),
            Healing = new ElementData(nameof(Healing), "Heal", new Color(0.78f, 0.49f, 0.71f));

        private ElementData(string name, Color color)
            : this(name, name, color)
        {
        }

        private ElementData(string name, string abbreviation, Color color)
        {
            Name = name;
            Abbreviation = abbreviation;
            Color = color;
        }

        public string Name { get; }
        public string Abbreviation { get; }
        public Color Color { get; }
    }

    public static class ElementExtensions
    {
        public static ElementData Get(this Element e) => ElementData.GetData(e);
    }
}
