using System.Collections.Generic;
using System.Linq;
using Arenbee.Statistics;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ItemStatsDisplay : PanelContainer
{
    private StatContainer _hpContainer = null!;
    private StatContainer _mpContainer = null!;
    private StatContainer _attackContainer = null!;
    private StatContainer _defenseContainer = null!;
    private StatContainer _mAttackContainer = null!;
    private StatContainer _mDefenseContainer = null!;
    private ElementContainer _elementAtkContainer = null!;
    private ElementContainer _elementDefContainer = null!;

    public override void _Ready()
    {
        base._Ready();
        GridContainer gridContainer = GetNode<GridContainer>("VBoxContainer/GridContainer");
        _hpContainer = gridContainer.GetNode<StatContainer>("HP");
        _mpContainer = gridContainer.GetNode<StatContainer>("MP");
        _attackContainer = gridContainer.GetNode<StatContainer>("Attack");
        _defenseContainer = gridContainer.GetNode<StatContainer>("Defense");
        _mAttackContainer = gridContainer.GetNode<StatContainer>("M Attack");
        _mDefenseContainer = gridContainer.GetNode<StatContainer>("M Defense");
        _elementAtkContainer = GetNode<ElementContainer>("VBoxContainer/EAtk");
        _elementDefContainer = GetNode<ElementContainer>("VBoxContainer/EDef");
        UpdateTypes();
    }

    public void UpdateTypes()
    {
        _hpContainer.UpdateType(AttributeType.MaxHP);
        _mpContainer.UpdateType(AttributeType.MaxMP);
        _attackContainer.UpdateType(AttributeType.Attack);
        _defenseContainer.UpdateType(AttributeType.Defense);
        _mAttackContainer.UpdateType(AttributeType.MagicAttack);
        _mDefenseContainer.UpdateType(AttributeType.MagicDefense);
        _elementAtkContainer.UpdateType(StatCategory.AttackElement);
        _elementDefContainer.UpdateType(StatCategory.ElementResist);
    }

    public void UpdateStatsDisplay(AItem? item)
    {
        List<Modifier>? mods = item?.Modifiers?.Where(x => !x.IsHidden).ToList();
        if (mods == null)
            return;
        _hpContainer.UpdateValue(mods);
        _mpContainer.UpdateValue(mods);
        _attackContainer.UpdateValue(mods);
        _defenseContainer.UpdateValue(mods);
        _mAttackContainer.UpdateValue(mods);
        _mDefenseContainer.UpdateValue(mods);
        _elementAtkContainer.UpdateValue(mods);
        _elementDefContainer.UpdateValue(mods);
    }
}
