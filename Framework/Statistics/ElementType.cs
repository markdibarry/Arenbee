using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public enum ElementType
    {
        None, Fire, Water, Wind, Earth, Plant, Dark, Light, Healing
    }

    public sealed class ElementData : Enum<ElementType, ElementData>
    {
        public static readonly ElementData
            None = new(nameof(None), new Color(1f, 1f, 1f)),
            Fire = new(nameof(Fire), new Color(0.78f, 0.12f, 0.06f)),
            Water = new(nameof(Water), new Color(0.2f, 0.25f, 0.78f)),
            Wind = new(nameof(Wind), new Color(0.27f, 0.78f, 0.59f)),
            Earth = new(nameof(Earth), new Color(0.71f, 0.51f, 0.2f)),
            Plant = new(nameof(Plant), new Color(0.33f, 0.73f, 0)),
            Dark = new(nameof(Dark), new Color(0.33f, 0.33f, 0.39f)),
            Light = new(nameof(Light), new Color(0.69f, 0.67f, 0.78f)),
            Healing = new(nameof(Healing), "Heal", new Color(0.78f, 0.49f, 0.71f));

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
        public static ElementData Get(this ElementType e) => ElementData.GetData(e);
    }
}
