using Arenbee.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class PointContainer : StatContainer
{
    private int _maxBaseValue;
    private StatType _maxStatType;
    private string _statMaxValueText = string.Empty;

    [Export]
    public string MaxText
    {
        get => _statMaxValueText;
        set
        {
            _statMaxValueText = value;
            if (MaxLabel != null)
                MaxLabel.Text = _statMaxValueText;
        }
    }
    public Label MaxLabel { get; set; } = null!;

    public override void _Ready()
    {
        base._Ready();
        MaxLabel = GetNode<Label>("%Max");
        MaxLabel.Text = _statMaxValueText;
    }

    public override void UpdateType(AttributeType attributeType)
    {
        StatType = StatTypeHelpers.GetStatType(attributeType);

        if (StatType == StatType.HP)
            _maxStatType = StatType.MaxHP;
        else if (StatType == StatType.MP)
            _maxStatType = StatType.MaxMP;

        UpdateNameLabel();
    }

    public override void UpdateBaseValue(Stats stats)
    {
        _maxBaseValue = stats.CalculateStat(_maxStatType);
    }

    public override void UpdateValue(Stats stats, bool updateColor)
    {
        int newMaxValue = stats.CalculateStat(_maxStatType);
        Text = stats.CalculateStat(StatType).ToString();
        MaxText = newMaxValue.ToString();
        if (updateColor)
        {
            Dim = _maxBaseValue == newMaxValue;
            DisplayValueColor(MaxLabel, newMaxValue);
            return;
        }
        Dim = false;
    }

    private void DisplayValueColor(Label label, int newValue)
    {
        if (newValue > _maxBaseValue)
            label.Modulate = GameCore.Colors.TextGreen;
        else if (newValue < _maxBaseValue)
            label.Modulate = GameCore.Colors.TextRed;
        else
            label.Modulate = Colors.White;
    }
}
