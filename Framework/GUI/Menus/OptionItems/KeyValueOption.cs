﻿using Arenbee.Framework.GUI;
using Godot;

[Tool]
public partial class KeyValueOption : OptionItem
{
    public static new readonly string ScenePath = $"res://Framework/GUI/Menus/OptionItems/{nameof(KeyValueOption)}.tscn";
    [Export(PropertyHint.MultilineText)]
    public string KeyText
    {
        get { return _keyText; }
        set
        {
            _keyText = value;
            if (_keyLabel != null)
            {
                _keyLabel.Text = _keyText;
            }
        }
    }
    [Export(PropertyHint.MultilineText)]
    public string ValueText
    {
        get { return _valueText; }
        set
        {
            _valueText = value;
            if (_valueLabel != null)
            {
                _valueLabel.Text = _valueText;
            }
        }
    }
    private Label _keyLabel;
    private string _keyText = string.Empty;
    private Label _valueLabel;
    private string _valueText = string.Empty;

    public override void _Ready()
    {
        _keyLabel = GetNodeOrNull<Label>("HBoxContainer/Key");
        _keyLabel.Text = _keyText;
        _valueLabel = GetNodeOrNull<Label>("HBoxContainer/Value");
        _valueLabel.Text = _valueText;
    }
}