using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;

[Tool]
public partial class KeyValueOption : OptionItem
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private string _keyText = string.Empty;
    private string _valueText = string.Empty;
    [Export(PropertyHint.MultilineText)]
    public string KeyText
    {
        get => _keyText;
        set
        {
            _keyText = value;
            if (KeyLabel != null)
                KeyLabel.Text = _keyText;
        }
    }
    [Export(PropertyHint.MultilineText)]
    public string ValueText
    {
        get => _valueText;
        set
        {
            _valueText = value;
            if (ValueLabel != null)
                ValueLabel.Text = _valueText;
        }
    }
    public Label KeyLabel { get; private set; }
    public Label ValueLabel { get; private set; }

    public override void _Ready()
    {
        KeyLabel = GetNodeOrNull<Label>("HBoxContainer/Key");
        KeyLabel.Text = _keyText;
        ValueLabel = GetNodeOrNull<Label>("HBoxContainer/Value");
        ValueLabel.Text = _valueText;
    }
}
