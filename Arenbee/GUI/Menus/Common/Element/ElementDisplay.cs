using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ElementDisplay : MarginContainer
{
    private TextureRect _elementColor;
    private ElementType _element;
    [Export]
    public ElementType Element
    {
        get { return _element; }
        set
        {
            _element = value;
            SetElementColor();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _elementColor = GetNode<TextureRect>("ElementColor");
        SetElementColor();
    }

    private void SetElementColor()
    {
        if (_elementColor != null)
            _elementColor.Modulate = _element.Get().Color;
    }
}
