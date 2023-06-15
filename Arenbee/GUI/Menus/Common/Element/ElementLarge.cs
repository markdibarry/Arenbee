using System;
using Arenbee.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ElementLarge : ElementDisplay
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private Sprite2D _effectivenessSprite = null!;
    private int _effectiveness = ElementResist.None;
    public int Effectiveness
    {
        get => _effectiveness;
        set
        {
            _effectiveness = value;
            DisplayEffectiveness(value);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _effectivenessSprite = GetNode<Sprite2D>("Effectiveness");
        DisplayEffectiveness(_effectiveness);
    }

    public void DisplayEffectiveness(int value)
    {
        if (_effectivenessSprite == null || _effectiveness == ElementResist.None)
            return;
        _effectivenessSprite.Show();
        Math.Clamp(value, ElementResist.Absorb, ElementResist.VeryWeak);

        switch (value)
        {
            case ElementResist.VeryWeak:
                _effectivenessSprite.Frame = 0;
                _effectivenessSprite.Modulate = GameCore.Colors.TextRed;
                break;
            case ElementResist.Weak:
                _effectivenessSprite.Frame = 0;
                break;
            case ElementResist.Resist:
                _effectivenessSprite.Frame = 1;
                break;
            case ElementResist.Nullify:
                _effectivenessSprite.Frame = 2;
                _effectivenessSprite.Modulate = GameCore.Colors.DimGrey;
                break;
            case ElementResist.Absorb:
                _effectivenessSprite.Frame = 3;
                _effectivenessSprite.Modulate = GameCore.Colors.TextGreen;
                break;
        }
    }
}
