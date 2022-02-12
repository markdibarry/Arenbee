using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        private bool _dim;
        [Export]
        public string OptionValue { get; set; } = string.Empty;
        [Export]
        public bool Dim
        {
            get { return _dim; }
            set
            {
                Modulate = value ? Colors.White.Darkened(0.3f) : Colors.White;
                _dim = value;
            }
        }
    }
}
