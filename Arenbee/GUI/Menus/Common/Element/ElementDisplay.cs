using Arenbee.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ElementDisplay : MarginContainer
{
    private TextureRect _elementColor = null!;
    private ElementType _elementType;
    public int BaseEffectiveness { get; set; } = ElementResist.None;
    public ElementType BaseElementType { get; set; }
    [Export]
    public ElementType ElementType
    {
        get => _elementType;
        set
        {
            _elementType = value;
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
            _elementColor.Modulate = StatTypeHelpers.GetElementColor(_elementType);
    }
}
