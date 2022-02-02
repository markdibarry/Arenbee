using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class TextOption : OptionItem
    {
        public static new readonly string ScenePath = $"res://Framework/GUI/Menus/OptionItems/{nameof(TextOption)}.tscn";
        [Export(PropertyHint.MultilineText)]
        public string LabelText
        {
            get { return _labelText; }
            set
            {
                _labelText = value;
                if (_label != null)
                {
                    _label.Text = _labelText;
                }
            }
        }
        private Label _label;
        private string _labelText = string.Empty;

        public override void _Ready()
        {
            _label = GetNodeOrNull<Label>("Label");
            _label.Text = _labelText;
        }
    }
}
