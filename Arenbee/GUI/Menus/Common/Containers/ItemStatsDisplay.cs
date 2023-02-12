using System.Linq;
using GameCore.Extensions;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ItemStatsDisplay : PanelContainer
{
    private PackedScene _elementScene = GD.Load<PackedScene>(ElementLarge.GetScenePath());
    private GridContainer _gridContainer = null!;
    private ElementContainer _elementAtkContainer = null!;
    private ElementContainer _elementDefContainer = null!;
    private StatContainer _hpContainer = null!;
    private StatContainer _mpContainer = null!;
    private StatContainer _attackContainer = null!;
    private StatContainer _defenseContainer = null!;
    private StatContainer _mAttackContainer = null!;
    private StatContainer _mDefenseContainer = null!;

    public override void _Ready()
    {
        base._Ready();
        _gridContainer = GetNode<GridContainer>("VBoxContainer/GridContainer");
        _hpContainer = _gridContainer.GetNode<StatContainer>("HP");
        _mpContainer = _gridContainer.GetNode<StatContainer>("MP");
        _attackContainer = _gridContainer.GetNode<StatContainer>("Attack");
        _defenseContainer = _gridContainer.GetNode<StatContainer>("Defense");
        _mAttackContainer = _gridContainer.GetNode<StatContainer>("M Attack");
        _mDefenseContainer = _gridContainer.GetNode<StatContainer>("M Defense");
        _elementAtkContainer = GetNode<ElementContainer>("VBoxContainer/EAtk");
        _elementDefContainer = GetNode<ElementContainer>("VBoxContainer/EDef");
    }

    public void UpdateStatsDisplay(AItem? item)
    {
        UpdateAttributes(item);
        UpdateEAtk(item);
        UpdateEDef(item);
    }

    private void UpdateAttributes(AItem? item)
    {
        var attMods = item?.Modifiers?.Where(x => !x.IsHidden && x.StatType == StatType.Attribute);
        _hpContainer.UpdateDisplay(attMods, AttributeType.MaxHP);
        _mpContainer.UpdateDisplay(attMods, AttributeType.MaxMP);
        _attackContainer.UpdateDisplay(attMods, AttributeType.Attack);
        _defenseContainer.UpdateDisplay(attMods, AttributeType.Defense);
        _mAttackContainer.UpdateDisplay(attMods, AttributeType.MagicAttack);
        _mDefenseContainer.UpdateDisplay(attMods, AttributeType.MagicDefense);
    }

    private void UpdateEAtk(AItem? item)
    {
        _elementAtkContainer.Elements.QueueFreeAllChildren();
        _elementAtkContainer.Dim = true;
        var eAtkMod = item?.Modifiers?.FirstOrDefault(x => !x.IsHidden && x.StatType == StatType.ElementOff);
        if (eAtkMod == null || (ElementType)eAtkMod.SubType == ElementType.None)
            return;
        var elementLg = _elementScene.Instantiate<ElementLarge>();
        elementLg.Element = (ElementType)eAtkMod.SubType;
        _elementAtkContainer.Elements.AddChild(elementLg);
        _elementAtkContainer.Dim = false;
    }

    private void UpdateEDef(AItem? item)
    {
        _elementDefContainer.Elements.QueueFreeAllChildren();
        _elementDefContainer.Dim = true;
        var mods = item?.Modifiers?.Where(x => !x.IsHidden && x.StatType == StatType.ElementDef);
        if (mods == null || !mods.Any())
            return;
        foreach (var mod in mods)
        {
            if (mod.Value == ElementDef.None)
                continue;
            var elementLg = _elementScene.Instantiate<ElementLarge>();
            elementLg.Element = (ElementType)mod.SubType;
            elementLg.Effectiveness = mod.Value;
            _elementDefContainer.Elements.AddChild(elementLg);
        }
        _elementDefContainer.Dim = false;
    }
}
