﻿using Godot;

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
                return (float)_colorShader.GetShaderParameter("_brightness");
            return 0;
        }
        set => _colorShader?.SetShaderParameter("_brightness", value);
    }
    [Export(PropertyHint.Range, "-1,1")]
    public float Contrast
    {
        get
        {
            if (_colorShader != null)
                return (float)_colorShader.GetShaderParameter("_contrast");
            return 1;
        }
        set => _colorShader?.SetShaderParameter("_contrast", value);
    }
    [Export(PropertyHint.Range, "-1,2")]
    public float Saturation
    {
        get
        {
            if (_colorShader != null)
                return (float)_colorShader.GetShaderParameter("_saturation");
            return 1;
        }
        set => _colorShader?.SetShaderParameter("_saturation", value);
    }
    [Export(PropertyHint.ColorNoAlpha)]
    public Color TintColor
    {
        get
        {
            if (_colorShader != null)
                return (Color)_colorShader.GetShaderParameter("_tint_color");
            return Colors.White;
        }
        set => _colorShader?.SetShaderParameter("_tint_color", value);
    }
    [Export(PropertyHint.Range, "0,1")]
    public float TintAmount
    {
        get
        {
            if (_colorShader != null)
                return (float)_colorShader.GetShaderParameter("_tint_amount");
            return 0;
        }
        set => _colorShader?.SetShaderParameter("_tint_amount", value);
    }

    public override void _Ready()
    {
        var rect = GetNodeOrNull<ColorRect>("ColorRect");
        _colorShader = rect.Material as ShaderMaterial;
    }
}