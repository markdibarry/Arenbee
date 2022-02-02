using Arenbee.Framework.GUI;
using Godot;
[Tool]
public partial class KeyValueOption : OptionItem
{
    private string _keyText = string.Empty;
    [Export(PropertyHint.MultilineText)]
    public string KeyText
    {
        get { return _keyText; }
        set
        {
            _keyText = value;
            if (KeyLabel != null)
            {
                KeyLabel.Text = _keyText;
            }
        }
    }
    private Label KeyLabel { get; set; }
    private string _valueText = string.Empty;
    [Export(PropertyHint.MultilineText)]
    public string ValueText
    {
        get { return _valueText; }
        set
        {
            _valueText = value;
            if (ValueLabel != null)
            {
                ValueLabel.Text = _valueText;
            }
        }
    }
    private Label ValueLabel { get; set; }

    public override void _Ready()
    {
        KeyLabel = GetNodeOrNull<Label>("HBoxContainer/Key");
        KeyLabel.Text = _keyText;
        ValueLabel = GetNodeOrNull<Label>("HBoxContainer/Value");
        ValueLabel.Text = _valueText;
    }
}
