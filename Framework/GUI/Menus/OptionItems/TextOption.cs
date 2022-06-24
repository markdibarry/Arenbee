using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class TextOption : OptionItem
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private Label _label;
        private string _labelText = string.Empty;
        [Export(PropertyHint.MultilineText)]
        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;
                if (_label != null)
                    _label.Text = _labelText;
            }
        }

        public override void _Ready()
        {
            _label = GetNodeOrNull<Label>("Label");
            _label.Text = _labelText;
        }
    }
}
