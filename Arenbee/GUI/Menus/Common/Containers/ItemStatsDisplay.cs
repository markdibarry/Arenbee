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
    private GridContainer _gridContainer = null!;
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
        _gridContainer = GetNode<GridContainer>("VBoxContainer/GridContainer");
        _hpContainer = _gridContainer.GetNode<StatContainer>("HP");
        _mpContainer = _gridContainer.GetNode<StatContainer>("MP");
        _attackContainer = _gridContainer.GetNode<StatContainer>("Attack");
        _defenseContainer = _gridContainer.GetNode<StatContainer>("Defense");
        _mAttackContainer = _gridContainer.GetNode<StatContainer>("MAttack");
        _mDefenseContainer = _gridContainer.GetNode<StatContainer>("MDefense");
        _elementAtkContainer = GetNode<ElementContainer>("VBoxContainer/EAtk");
        _elementDefContainer = GetNode<ElementContainer>("VBoxContainer/EDef");
        SetLabelContainer(_hpContainer);
        SetLabelContainer(_mpContainer);
        SetLabelContainer(_attackContainer);
        SetLabelContainer(_defenseContainer);
        SetLabelContainer(_mAttackContainer);
        SetLabelContainer(_mDefenseContainer);
        UpdateTypes();
    }

    private void SetLabelContainer(StatContainer statContainer)
    {
        LabelContainer label = _gridContainer.GetNode<LabelContainer>(statContainer.Name + "Label");
        statContainer.SetLabelContainer(label);
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
        List<Modifier> mods = item?.Modifiers?.Where(x => !x.IsHidden).ToList() ?? new();
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
