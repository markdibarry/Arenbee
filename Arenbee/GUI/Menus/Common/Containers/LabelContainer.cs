using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class LabelContainer : MarginContainer
{
    protected bool _dim;
    private Label _label = null!;
    private string _labelText = string.Empty;
    [Export]
    public virtual bool Dim
    {
        get => _dim;
        set
        {
            _dim = value;
            Modulate = _dim ? GameCore.Colors.DimGrey : Colors.White;
        }
    }
    [Export]
    public string Text
    {
        get => _labelText;
        set
        {
            _labelText = value;
            if (Label != null)
                Label.Text = _labelText;
        }
    }
    public Label Label { get; private set; } = null!;

    public override void _Ready()
    {
        Label = GetNode<Label>("%Label");
        Label.Text = _labelText;
    }
}
