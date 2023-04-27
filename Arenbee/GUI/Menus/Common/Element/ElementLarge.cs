using Arenbee.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ElementLarge : ElementDisplay
{
    public ElementLarge()
    {
        Effectiveness = ElementResist.None;
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private Sprite2D _effectivenessSprite = null!;
    public int Effectiveness { get; set; }


    public override void _Ready()
    {
        base._Ready();
        _effectivenessSprite = GetNode<Sprite2D>("Effectiveness");
        SetEffectiveness(Effectiveness);
    }

    public void SetEffectiveness(int value)
    {
        if (Effectiveness == ElementResist.None) return;
        _effectivenessSprite.Show();
        if (value > ElementResist.VeryWeak)
            value = ElementResist.VeryWeak;
        else if (value < ElementResist.Absorb)
            value = ElementResist.Absorb;

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
