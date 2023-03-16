using Arenbee.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ActorStatsDisplay : PanelContainer
{
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
        base._Ready();
        _levelContainer = GetNode<StatContainer>("%Level");
        _hpContainer = GetNode<PointContainer>("%HP");
        _mpContainer = GetNode<PointContainer>("%MP");
        _attackContainer = GetNode<StatContainer>("%Attack");
        _defenseContainer = GetNode<StatContainer>("%Defense");
        _mAttackContainer = GetNode<StatContainer>("%M Attack");
        _mDefenseContainer = GetNode<StatContainer>("%M Defense");
        _elAttackContainer = GetNode<ElementContainer>("%EAtk");
        _elResistContainer = GetNode<ElementContainer>("%EDef");
        UpdateTypes();
    }

    public void UpdateTypes()
    {
        _levelContainer.UpdateType(AttributeType.Level);
        _hpContainer.UpdateType(AttributeType.MaxHP);
        _mpContainer.UpdateType(AttributeType.MaxMP);
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

    public void UpdateStatsDisplay(Stats stats)
    {
        _levelContainer.UpdateValue(stats);
        _hpContainer.UpdateDisplay(stats);
        _mpContainer.UpdateDisplay(stats);
        _attackContainer.UpdateValue(stats);
        _defenseContainer.UpdateValue(stats);
        _mAttackContainer.UpdateValue(stats);
        _mDefenseContainer.UpdateValue(stats);
        _elAttackContainer.UpdateValue(stats);
        _elResistContainer.UpdateValue(stats);
    }
}
