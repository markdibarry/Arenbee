using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class ElementLarge : ElementDisplay
    {
        public ElementLarge()
        {
            Effectiveness = ElementDefense.None;
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
            if (Effectiveness == ElementDefense.None) return;
            _effectivenessSprite.Show();
            if (value > ElementDefense.VeryWeak)
                value = ElementDefense.VeryWeak;
            else if (value < ElementDefense.Absorb)
                value = ElementDefense.Absorb;

            switch (value)
            {
                case ElementDefense.VeryWeak:
                    _effectivenessSprite.Frame = 0;
                    _effectivenessSprite.Modulate = ColorConstants.TextRed;
                    break;
                case ElementDefense.Weak:
                    _effectivenessSprite.Frame = 0;
                    break;
                case ElementDefense.Resist:
                    _effectivenessSprite.Frame = 1;
                    break;
                case ElementDefense.Nullify:
                    _effectivenessSprite.Frame = 2;
                    _effectivenessSprite.Modulate = ColorConstants.DimGrey;
                    break;
                case ElementDefense.Absorb:
                    _effectivenessSprite.Frame = 3;
                    _effectivenessSprite.Modulate = ColorConstants.TextGreen;
                    break;
            }
        }
    }
}