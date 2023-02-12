using System.Collections.Generic;
using System.Linq;
using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class StatContainer : EqualContainer
{
    public static new string GetScenePath() => GDEx.GetScenePath();
    private bool _dim;
    private string _statNameText = string.Empty;
    private string _statValueText = string.Empty;
    [Export]
    public string StatNameText
    {
        get => _statNameText;
        set
        {
            _statNameText = value;
            if (StatNameLabel != null)
                StatNameLabel.Text = _statNameText;
        }
    }
    [Export]
    public string StatValueText
    {
        get => _statValueText;
        set
        {
            _statValueText = value;
            if (StatValueLabel != null)
                StatValueLabel.Text = _statValueText;
        }
    }
    public Label StatNameLabel { get; private set; } = null!;
    public Label StatValueLabel { get; private set; } = null!;
    [Export]
    public bool Dim
    {
        get => _dim;
        set
        {
            _dim = value;
            Modulate = _dim ? GameCore.Colors.DimGrey : Colors.White;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        StatNameLabel = GetNode<Label>("%Key");
        StatValueLabel = GetNode<Label>("%Value");
        StatNameLabel.Text = _statNameText;
        StatValueLabel.Text = _statValueText;
        StatNameLabel.Resized += OnResize;
        StatValueLabel.Resized += OnResize;
    }

    public override void OnResize()
    {
        ResizeItems(StatNameLabel, StatValueLabel);
    }

    public void UpdateDisplay(IEnumerable<Modifier> mods, AttributeType attributeType)
    {
        Dim = true;
        StatNameText = attributeType.Get().Abbreviation + ":";
        StatValueText = "0";
        var mod = mods?.FirstOrDefault(x => (AttributeType)x.SubType == attributeType);
        var value = mod != null ? mod.Value : 0;
        StatValueText = value.ToString();
        if (value != 0)
            Dim = false;
        DisplayValueColor(0, value);
    }

    public void UpdateDisplay(Stats? oldStats, Stats newStats, AttributeType attributeType)
    {
        int? oldValue = oldStats?.Attributes.GetStat(attributeType)?.DisplayValue;
        int newValue = newStats.Attributes.GetStat(attributeType)?.DisplayValue ?? default;
        StatNameText = attributeType.Get().Abbreviation + ":";
        StatValueText = newValue.ToString();
        Dim = oldStats != null && oldValue == newValue;
        DisplayValueColor(oldValue, newValue);
    }

    private void DisplayValueColor(int? oldValue, int newValue)
    {
        if (oldValue == null)
        {
            StatValueLabel.Modulate = Colors.White;
            return;
        }

        if (newValue > oldValue)
            StatValueLabel.Modulate = GameCore.Colors.TextGreen;
        else if (newValue < oldValue)
            StatValueLabel.Modulate = GameCore.Colors.TextRed;
        else
            StatValueLabel.Modulate = Colors.White;
    }
}
