using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class TextOption : OptionItem
    {
        private string _labelText = string.Empty;
        [Export(PropertyHint.MultilineText)]
        public string LabelText
        {
            get { return _labelText; }
            set
            {
                _labelText = value;
                if (Label != null)
                {
                    Label.Text = _labelText;
                }
            }
        }
        public Label Label { get; set; }

        public override void _Ready()
        {
            Label = GetNodeOrNull<Label>("Label");
            Label.Text = _labelText;
        }
    }
}
