using GameCore.Constants;
using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common
{
    [Tool]
    public partial class ElementLarge : ElementDisplay
    {
        public ElementLarge()
        {
            Effectiveness = ElementDef.None;
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        public int Effectiveness { get; set; }
        private Sprite2D _effectivenessSprite;

        public override void _Ready()
        {
            base._Ready();
            _effectivenessSprite = GetNode<Sprite2D>("Effectiveness");
            SetEffectiveness(Effectiveness);
        }

        public void SetEffectiveness(int value)
        {
            if (Effectiveness == ElementDef.None) return;
            _effectivenessSprite.Show();
            if (value > ElementDef.VeryWeak)
                value = ElementDef.VeryWeak;
            else if (value < ElementDef.Absorb)
                value = ElementDef.Absorb;

            switch (value)
            {
                case ElementDef.VeryWeak:
                    _effectivenessSprite.Frame = 0;
                    _effectivenessSprite.Modulate = ColorConstants.TextRed;
                    break;
                case ElementDef.Weak:
                    _effectivenessSprite.Frame = 0;
                    break;
                case ElementDef.Resist:
                    _effectivenessSprite.Frame = 1;
                    break;
                case ElementDef.Nullify:
                    _effectivenessSprite.Frame = 2;
                    _effectivenessSprite.Modulate = ColorConstants.DimGrey;
                    break;
                case ElementDef.Absorb:
                    _effectivenessSprite.Frame = 3;
                    _effectivenessSprite.Modulate = ColorConstants.TextGreen;
                    break;
            }
        }
    }
}