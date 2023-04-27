using Arenbee.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ActorStatsDisplay : PanelContainer
{
    private GridContainer _gridContainer = null!;
    private StatContainer _levelContainer = null!;
    private PointContainer _hpContainer = null!;
    private PointContainer _mpContainer = null!;
    private StatContainer _attackContainer = null!;
    private StatContainer _defenseContainer = null!;
    private StatContainer _mAttackContainer = null!;
    private StatContainer _mDefenseContainer = null!;
    private ElementContainer _elAttackContainer = null!;
    private ElementContainer _elResistContainer = null!;

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("VBoxContainer/GridContainer");
        _levelContainer = _gridContainer.GetNode<StatContainer>("Level");
        _hpContainer = _gridContainer.GetNode<PointContainer>("HP");
        _mpContainer = _gridContainer.GetNode<PointContainer>("MP");
        _attackContainer = _gridContainer.GetNode<StatContainer>("Attack");
        _defenseContainer = _gridContainer.GetNode<StatContainer>("Defense");
        _mAttackContainer = _gridContainer.GetNode<StatContainer>("MAttack");
        _mDefenseContainer = _gridContainer.GetNode<StatContainer>("MDefense");
        _elAttackContainer = GetNode<ElementContainer>("%EAtk");
        _elResistContainer = GetNode<ElementContainer>("%EDef");
        SetLabelContainer(_levelContainer);
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
        _levelContainer.UpdateType(AttributeType.Level);
        _hpContainer.UpdateType(AttributeType.HP);
        _mpContainer.UpdateType(AttributeType.MP);
        _attackContainer.UpdateType(AttributeType.Attack);
        _defenseContainer.UpdateType(AttributeType.Defense);
        _mAttackContainer.UpdateType(AttributeType.MagicAttack);
        _mDefenseContainer.UpdateType(AttributeType.MagicDefense);
        _elAttackContainer.UpdateType(StatCategory.AttackElement);
        _elResistContainer.UpdateType(StatCategory.ElementResist);
    }

    public void UpdateBaseValues(Stats stats)
    {
        _levelContainer.UpdateBaseValue(stats);
        _hpContainer.UpdateBaseValue(stats);
        _mpContainer.UpdateBaseValue(stats);
        _attackContainer.UpdateBaseValue(stats);
        _defenseContainer.UpdateBaseValue(stats);
        _mAttackContainer.UpdateBaseValue(stats);
        _mDefenseContainer.UpdateBaseValue(stats);
        _elAttackContainer.UpdateBaseValue(stats);
        _elResistContainer.UpdateBaseValue(stats);
    }

    public void UpdateStatsDisplay(Stats stats, bool updateColor)
    {
        _levelContainer.UpdateValue(stats, updateColor);
        _hpContainer.UpdateValue(stats, updateColor);
        _mpContainer.UpdateValue(stats, updateColor);
        _attackContainer.UpdateValue(stats, updateColor);
        _defenseContainer.UpdateValue(stats, updateColor);
        _mAttackContainer.UpdateValue(stats, updateColor);
        _mDefenseContainer.UpdateValue(stats, updateColor);
        _elAttackContainer.UpdateValue(stats, updateColor);
        _elResistContainer.UpdateValue(stats, updateColor);
    }
}
