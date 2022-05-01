using Godot;

namespace Arenbee.Framework.Utility
{
    [Tool]
    public partial class ColorAdjustment : CanvasLayer
    {
        private ShaderMaterial _colorShader;
        [Export(PropertyHint.Range, "-1,1")]
        public float Brightness
        {
            get
            {
                if (_colorShader != null)
                    return (float)_colorShader.GetShaderParam("_brightness");
                return 0;
            }
            set
            {
                _colorShader?.SetShaderParam("_brightness", value);
            }
        }
        [Export(PropertyHint.Range, "0,3")]
        public float Contrast
        {
            get
            {
                if (_colorShader != null)
                    return (float)_colorShader.GetShaderParam("_contrast");
                return 1;
            }
            set
            {
                _colorShader?.SetShaderParam("_contrast", value);
            }
        }
        [Export(PropertyHint.Range, "0,3")]
        public float Saturation
        {
            get
            {
                if (_colorShader != null)
                    return (float)_colorShader.GetShaderParam("_saturation");
                return 1;
            }
            set
            {
                _colorShader?.SetShaderParam("_saturation", value);
            }
        }
        [Export(PropertyHint.ColorNoAlpha)]
        public Color TintColor
        {
            get
            {
                if (_colorShader != null)
                    return (Color)_colorShader.GetShaderParam("_tint_color");
                return Colors.White;
            }
            set
            {
                _colorShader?.SetShaderParam("_tint_color", value);
            }
        }
        [Export(PropertyHint.Range, "0,1")]
        public float TintAmount
        {
            get
            {
                if (_colorShader != null)
                    return (float)_colorShader.GetShaderParam("_tint_amount");
                return 0;
            }
            set
            {
                _colorShader?.SetShaderParam("_tint_amount", value);
            }
        }

        public override void _Ready()
        {
            var rect = GetNodeOrNull<ColorRect>("ColorRect");
            _colorShader = rect.Material as ShaderMaterial;
        }
    }
}
