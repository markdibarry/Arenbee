using System.Collections.Generic;
using Arenbee.Statistics;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class StatContainer : LabelContainer
{
    public override bool Dim
    {
        get => _dim;
        set
        {
            _dim = value;
            Modulate = _dim ? GameCore.Colors.DimGrey : Colors.White;
            if (NameContainer != null)
                NameContainer.Dim = _dim;
        }
    }
    protected int BaseValue { get; set; }
    protected StatType StatType { get; set; }
    protected LabelContainer? NameContainer { get; private set; }

    public void SetLabelContainer(LabelContainer textContainer)
    {
        NameContainer = textContainer;
    }

    public void UpdateNameLabel()
    {
        if (NameContainer != null)
            NameContainer.Text = this.TrS(StatTypeDB.GetStatTypeData(StatType).Abbreviation) + ":";
    }

    public virtual void UpdateType(AttributeType attributeType)
    {
        StatType = StatTypeHelpers.GetStatType(attributeType);
        UpdateNameLabel();
    }

    public virtual void UpdateBaseValue(Stats stats)
    {
        BaseValue = stats.CalculateStat(StatType);
    }

    public void UpdateValue(List<Modifier> mods)
    {
        int value = 0;
        foreach (Modifier mod in mods)
        {
            if (mod.StatType != (int)StatType)
                continue;
            if (mod.Op == ModOp.Add)
                value += mod.Value;
            else if (mod.Op == ModOp.Subtract)
                value -= mod.Value;
        }
        Text = value.ToString();
        Dim = value == 0;
        DisplayValueColor(Label, value);
    }

    public virtual void UpdateValue(Stats stats, bool updateColor)
    {
        int newValue = stats.CalculateStat(StatType);
        Text = newValue.ToString();
        if (updateColor)
        {
            Dim = BaseValue == newValue;
            DisplayValueColor(Label, newValue);
            return;
        }
        Dim = false;
    }

    private void DisplayValueColor(Label label, int newValue)
    {
        if (newValue > BaseValue)
            label.Modulate = GameCore.Colors.TextGreen;
        else if (newValue < BaseValue)
            label.Modulate = GameCore.Colors.TextRed;
        else
            label.Modulate = Colors.White;
    }
}
