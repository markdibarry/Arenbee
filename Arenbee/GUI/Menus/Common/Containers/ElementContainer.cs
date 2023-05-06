using System.Collections.Generic;
using System.Linq;
using Arenbee.Statistics;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ElementContainer : HBoxContainer
{
    private PackedScene _elementScene = GD.Load<PackedScene>(ElementLarge.GetScenePath());
    private bool _dim;
    private string _statNameText = string.Empty;
    private StatCategory _statCategory;

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
    public HBoxContainer Elements { get; set; } = null!;
    public Label StatNameLabel { get; set; } = null!;
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

    public override void _Ready()
    {
        StatNameLabel = GetNode<Label>("Key");
        StatNameLabel.Text = _statNameText;
        Elements = GetNode<HBoxContainer>("Elements");
    }

    public void UpdateType(StatCategory category)
    {
        Elements.QueueFreeAllChildren();
        StatNameText = this.TrS(StatTypeDB.GetStatCategoryData(category).Abbreviation) + ":";
        _statCategory = category;
        if (category == StatCategory.AttackElement)
        {
            var elementLg = _elementScene.Instantiate<ElementLarge>();
            Elements.AddChild(elementLg);
            elementLg.Hide();
        }
        else if (category == StatCategory.ElementResist)
        {
            foreach (ElementType element in GDEx.GetEnums<ElementType>())
            {
                if (element == ElementType.None)
                    continue;
                var elementLg = _elementScene.Instantiate<ElementLarge>();
                elementLg.ElementType = element;
                Elements.AddChild(elementLg);
                elementLg.Hide();
            }
        }
    }

    public void UpdateBaseValue(Stats stats)
    {
        if (_statCategory == StatCategory.AttackElement)
            UpdateBaseElementAttack(stats);
        else if (_statCategory == StatCategory.ElementResist)
            UpdateBaseElementResist(stats);
    }

    public void UpdateValue(Stats stats, bool updateColor)
    {
        if (_statCategory == StatCategory.AttackElement)
            UpdateElementAttack(stats, updateColor);
        else if (_statCategory == StatCategory.ElementResist)
            UpdateElementResist(stats, updateColor);
    }

    public void UpdateValue(List<Modifier> modifiers)
    {
        if (_statCategory == StatCategory.AttackElement)
            UpdateElementAttack(modifiers);
        else if (_statCategory == StatCategory.ElementResist)
            UpdateElementResist(modifiers);
    }

    private void UpdateBaseElementAttack(Stats stats)
    {
        ElementLarge lgElement = Elements.GetChild<ElementLarge>(0);
        ElementType elementType = (ElementType)stats.CalculateStat(StatType.AttackElement);
        lgElement.BaseElementType = elementType;
    }

    private void UpdateBaseElementResist(Stats stats)
    {
        IEnumerable<ElementLarge> elements = Elements.GetChildren<ElementLarge>();
        foreach (ElementLarge element in elements)
        {
            StatType statType = StatTypeHelpers.GetElementResist(element.ElementType);
            int effectiveness = stats.CalculateStat(statType);
            element.BaseEffectiveness = effectiveness;
        }
    }

    private void UpdateElementAttack(List<Modifier> modifiers)
    {
        Dim = true;
        ElementLarge lgElement = Elements.GetChild<ElementLarge>(0);
        lgElement.Hide();
        var atkElMods = modifiers.Where(x => StatTypeHelpers.GetStatCategory(x.StatType) == StatCategory.AttackElement);
        if (!atkElMods.Any())
            return;
        foreach (Modifier modifier in atkElMods)
            lgElement.ElementType = (ElementType)modifier.Value;
        lgElement.Show();
        Dim = false;
    }

    private void UpdateElementAttack(Stats stats, bool updateColor)
    {
        ElementLarge lgElement = Elements.GetChild<ElementLarge>(0);
        ElementType elementType = (ElementType)stats.CalculateStat(StatType.AttackElement);
        lgElement.ElementType = elementType;
        lgElement.Visible = elementType != ElementType.None;
        Dim = updateColor && elementType == lgElement.BaseElementType;
    }

    private void UpdateElementResist(List<Modifier> modifiers)
    {
        Dim = true;
        IEnumerable<ElementLarge> elements = Elements.GetChildren<ElementLarge>();
        foreach (ElementLarge element in elements)
            element.Hide();
        var resistMods = modifiers.Where(x => StatTypeHelpers.GetStatCategory(x.StatType) == StatCategory.ElementResist);
        if (!resistMods.Any())
            return;
        foreach (Modifier modifier in resistMods)
        {
            ElementType elementType = StatTypeHelpers.GetElement((StatType)modifier.StatType);
            ElementLarge element = elements.First(x => x.ElementType == elementType);
            element.Effectiveness = modifier.Value;
            element.Show();
        }
        Dim = false;
    }

    private void UpdateElementResist(Stats stats, bool updateColor)
    {
        IEnumerable<ElementLarge> elements = Elements.GetChildren<ElementLarge>();
        bool changed = false;
        foreach (ElementLarge element in elements)
        {
            StatType statType = StatTypeHelpers.GetElementResist(element.ElementType);
            int effectiveness = stats.CalculateStat(statType);
            element.Effectiveness = effectiveness;
            if (effectiveness != element.BaseEffectiveness)
                changed = true;
            element.Visible = effectiveness != ElementResist.None;
        }
        Dim = updateColor && !changed;
    }
}
