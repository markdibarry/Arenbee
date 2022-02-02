using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        [Export]
        public string Value { get; set; } = string.Empty;
    }
}
