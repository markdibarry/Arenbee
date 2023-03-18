using Arenbee.Statistics;
using GameCore.Extensions;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class PointContainer : EqualContainer
{
    public static new string GetScenePath() => GDEx.GetScenePath();
    private bool _dim;
    private int _maxBaseValue;
    private StatType _statType;
    private StatType _maxStatType;
    private string _statNameText = string.Empty;
    private string _statCurrentValueText = string.Empty;
    private string _statMaxValueText = string.Empty;
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
    public string StatCurrentValueText
    {
        get => _statCurrentValueText;
        set
        {
            _statCurrentValueText = value;
            if (StatCurrentValueLabel != null)
                StatCurrentValueLabel.Text = _statCurrentValueText;
        }
    }
    [Export]
    public string StatMaxValueText
    {
        get => _statMaxValueText;
        set
        {
            _statMaxValueText = value;
            if (StatMaxValueLabel != null)
                StatMaxValueLabel.Text = _statMaxValueText;
        }
    }
    public Label StatNameLabel { get; set; } = null!;
    public HBoxContainer ValueHBox { get; set; } = null!;
    public Label StatCurrentValueLabel { get; set; } = null!;
    public Label StatMaxValueLabel { get; set; } = null!;

    public override void _Ready()
    {
        base._Ready();
        ValueHBox = ValueContainer.GetNode<HBoxContainer>("HBoxContainer");
        StatNameLabel = GetNode<Label>("%Key");
        StatCurrentValueLabel = GetNode<Label>("%Current");
        StatMaxValueLabel = GetNode<Label>("%Max");
        StatNameLabel.Text = _statNameText;
        StatCurrentValueLabel.Text = _statCurrentValueText;
        StatMaxValueLabel.Text = _statMaxValueText;
        StatNameLabel.Resized += OnResize;
        ValueHBox.Resized += OnResize;
    }

    public override void OnResize()
    {
        ResizeItems(StatNameLabel, ValueHBox);
    }

    public void UpdateType(AttributeType attributeType)
    {
        _statType = StatTypeHelpers.GetStatType(attributeType);
        if (_statType == StatType.HP)
            _maxStatType = StatType.MaxHP;
        else if (_statType == StatType.MP)
            _maxStatType = StatType.MaxMP;
        StatNameText = Tr(StatTypeDB.GetStatTypeData(_statType).Abbreviation) + ":";
    }

    public void UpdateBaseValue(Stats stats)
    {
        _maxBaseValue = stats.CalculateStat(_maxStatType);
    }

    public void UpdateDisplay(Stats stats, bool updateColor)
    {
        int newMaxValue = stats.CalculateStat(_maxStatType);
        StatCurrentValueText = stats.CalculateStat(_statType).ToString();
        StatMaxValueText = newMaxValue.ToString();
        if (updateColor)
        {
            Dim = _maxBaseValue == newMaxValue;
            DisplayValueColor(newMaxValue);
            return;
        }
        Dim = false;
    }

    private void DisplayValueColor(int newValue)
    {
        if (newValue > _maxBaseValue)
            StatMaxValueLabel.Modulate = GameCore.Colors.TextGreen;
        else if (newValue < _maxBaseValue)
            StatMaxValueLabel.Modulate = GameCore.Colors.TextRed;
        else
            StatMaxValueLabel.Modulate = Colors.White;
    }
}
