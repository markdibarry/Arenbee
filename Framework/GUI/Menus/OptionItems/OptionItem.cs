using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        public static readonly string ScenePath = $"res://Framework/GUI/Menus/OptionItems/{nameof(OptionItem)}.tscn";
        [Export]
        public string OptionValue { get; set; } = string.Empty;
    }
}
