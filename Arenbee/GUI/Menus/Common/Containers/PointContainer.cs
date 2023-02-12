using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class PointContainer : EqualContainer
{
    public static new string GetScenePath() => GDEx.GetScenePath();
    private bool _dim;
    private string _statNameText;
    private string _statCurrentValueText;
    private string _statMaxValueText;
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
    public Label StatNameLabel { get; set; }
    public HBoxContainer ValueHBox { get; set; }
    public Label StatCurrentValueLabel { get; set; }
    public Label StatMaxValueLabel { get; set; }

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

    public void UpdateDisplay(Stats? oldStats, Stats newStats, AttributeType attributeType)
    {
        AttributeType maxType;
        if (attributeType == AttributeType.HP)
            maxType = AttributeType.MaxHP;
        else if (attributeType == AttributeType.MP)
            maxType = AttributeType.MaxMP;
        else
            return;
        int? oldValue = oldStats?.Attributes.GetStat(maxType)?.DisplayValue;
        int newValue = newStats.Attributes.GetStat(maxType)?.DisplayValue ?? default;
        StatNameText = attributeType.Get().Abbreviation + ":";
        StatCurrentValueText = newStats.Attributes.GetStat(attributeType)?.DisplayValue.ToString();
        StatMaxValueText = newValue.ToString();
        Dim = oldStats != null && oldValue == newValue;
        DisplayValueColor(oldValue, newValue);
    }

    private void DisplayValueColor(int? oldValue, int newValue)
    {
        if (oldValue == null)
        {
            StatMaxValueLabel.Modulate = Colors.White;
            return;
        }

        if (newValue > oldValue)
            StatMaxValueLabel.Modulate = GameCore.Colors.TextGreen;
        else if (newValue < oldValue)
            StatMaxValueLabel.Modulate = GameCore.Colors.TextRed;
        else
            StatMaxValueLabel.Modulate = Colors.White;
    }
}
