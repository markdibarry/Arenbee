using System.Collections.Generic;
using System.Linq;
using Arenbee.Statistics;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class StatContainer : EqualContainer
{
    public static new string GetScenePath() => GDEx.GetScenePath();
    private bool _dim;
    private int _baseValue;
    private StatType _statType;
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

    public void UpdateType(AttributeType attributeType)
    {
        _statType = StatTypeHelpers.GetStatType(attributeType);
        StatNameText = Tr(StatTypeDB.GetStatTypeData(_statType).Abbreviation) + ":";
    }

    public void UpdateBaseValue(Stats stats)
    {
        _baseValue = stats.CalculateStat(_statType);
    }

    public void UpdateValue(IEnumerable<Modifier> mods)
    {
        int value = 0;
        foreach (Modifier mod in mods)
        {
            if (mod.StatType != (int)_statType)
                continue;
            if (mod.Op == ModOp.Add)
                value += mod.Value;
            else if (mod.Op == ModOp.Subtract)
                value -= mod.Value;
        }
        StatValueText = value.ToString();
        Dim = value == 0;
        DisplayValueColor(value);
    }

    public void UpdateValue(Stats stats)
    {
        int newValue = stats.CalculateStat(_statType);
        StatValueText = newValue.ToString();
        Dim = _baseValue == newValue;
        DisplayValueColor(newValue);
    }

    private void DisplayValueColor(int newValue)
    {
        if (newValue > _baseValue)
            StatValueLabel.Modulate = GameCore.Colors.TextGreen;
        else if (newValue < _baseValue)
            StatValueLabel.Modulate = GameCore.Colors.TextRed;
        else
            StatValueLabel.Modulate = Colors.White;
    }
}
