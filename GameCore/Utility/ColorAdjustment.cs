using Godot;

namespace GameCore.Utility;

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
                return (float)_colorShader.GetShaderUniform("_brightness");
            return 0;
        }
        set => _colorShader?.SetShaderUniform("_brightness", value);
    }
    [Export(PropertyHint.Range, "-1,1")]
    public float Contrast
    {
        get
        {
            if (_colorShader != null)
                return (float)_colorShader.GetShaderUniform("_contrast");
            return 1;
        }
        set => _colorShader?.SetShaderUniform("_contrast", value);
    }
    [Export(PropertyHint.Range, "-1,2")]
    public float Saturation
    {
        get
        {
            if (_colorShader != null)
                return (float)_colorShader.GetShaderUniform("_saturation");
            return 1;
        }
        set => _colorShader?.SetShaderUniform("_saturation", value);
    }
    [Export(PropertyHint.ColorNoAlpha)]
    public Color TintColor
    {
        get
        {
            if (_colorShader != null)
                return (Color)_colorShader.GetShaderUniform("_tint_color");
            return Colors.White;
        }
        set => _colorShader?.SetShaderUniform("_tint_color", value);
    }
    [Export(PropertyHint.Range, "0,1")]
    public float TintAmount
    {
        get
        {
            if (_colorShader != null)
                return (float)_colorShader.GetShaderUniform("_tint_amount");
            return 0;
        }
        set => _colorShader?.SetShaderUniform("_tint_amount", value);
    }

    public override void _Ready()
    {
        var rect = GetNodeOrNull<ColorRect>("ColorRect");
        _colorShader = rect.Material as ShaderMaterial;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
            _colorShader.Dispose();
    }
}
