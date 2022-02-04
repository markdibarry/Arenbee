using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        public static readonly string ScenePath = $"res://Framework/GUI/Menus/OptionItems/{nameof(OptionItem)}.tscn";
        private bool _dim;
        [Export]
        public string OptionValue { get; set; } = string.Empty;
        [Export]
        public bool Dim
        {
            get { return _dim; }
            set
            {
                if (value && !_dim)
                    Modulate = Modulate.Darkened(0.3f);
                else
                    Modulate = new Color(Colors.White);
                _dim = value;
            }
        }
    }
}
