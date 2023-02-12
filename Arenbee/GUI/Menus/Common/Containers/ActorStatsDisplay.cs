using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class ActorStatsDisplay : PanelContainer
{
    private PackedScene _elementScene;
    private GridContainer _gridContainer;
    private ElementContainer _elementAtkContainer;
    private ElementContainer _elementDefContainer;
    private StatContainer _levelContainer;
    private PointContainer _hpContainer;
    private PointContainer _mpContainer;
    private StatContainer _attackContainer;
    private StatContainer _defenseContainer;
    private StatContainer _mAttackContainer;
    private StatContainer _mDefenseContainer;

    public override void _Ready()
    {
        base._Ready();
        _elementScene = GD.Load<PackedScene>(ElementLarge.GetScenePath());
        _levelContainer = GetNode<StatContainer>("%Level");
        _hpContainer = GetNode<PointContainer>("%HP");
        _mpContainer = GetNode<PointContainer>("%MP");
        _attackContainer = GetNode<StatContainer>("%Attack");
        _defenseContainer = GetNode<StatContainer>("%Defense");
        _mAttackContainer = GetNode<StatContainer>("%M Attack");
        _mDefenseContainer = GetNode<StatContainer>("%M Defense");
        _elementAtkContainer = GetNode<ElementContainer>("%EAtk");
        _elementDefContainer = GetNode<ElementContainer>("%EDef");
    }

    public void UpdateStatsDisplay(Stats? oldStats, Stats newStats)
    {
        if (newStats == null)
            return;
        _levelContainer.UpdateDisplay(oldStats, newStats, AttributeType.Level);
        _hpContainer.UpdateDisplay(oldStats, newStats, AttributeType.HP);
        _mpContainer.UpdateDisplay(oldStats, newStats, AttributeType.MP);
        _attackContainer.UpdateDisplay(oldStats, newStats, AttributeType.Attack);
        _defenseContainer.UpdateDisplay(oldStats, newStats, AttributeType.Defense);
        _mAttackContainer.UpdateDisplay(oldStats, newStats, AttributeType.MagicAttack);
        _mDefenseContainer.UpdateDisplay(oldStats, newStats, AttributeType.MagicDefense);
        UpdateEAtk(oldStats, newStats);
        UpdateEDef(oldStats, newStats);
    }

    private void UpdateEAtk(Stats? oldStats, Stats newStats)
    {
        _elementAtkContainer.Elements.QueueFreeAllChildren();
        var newElement = newStats.ElementOffs.CurrentElement;
        _elementAtkContainer.Dim = oldStats != null && oldStats.ElementOffs.CurrentElement == newElement;
        if (newElement != ElementType.None)
        {
            var elementLg = _elementScene.Instantiate<ElementLarge>();
            elementLg.Element = newElement;
            _elementAtkContainer.Elements.AddChild(elementLg);
        }
    }

    private void UpdateEDef(Stats? oldStats, Stats newStats)
    {
        _elementDefContainer.Elements.QueueFreeAllChildren();
        _elementDefContainer.Dim = true;
        bool changed = false;
        foreach (var element in Enum<ElementType>.Values())
        {
            var oldDef = oldStats?.ElementDefs.GetStat(element);
            var newDef = newStats.ElementDefs.GetStat(element);
            if (!ElementDef.Equals(oldDef, newDef))
                changed = true;
            if (newDef == null || newDef.ModifiedValue == ElementDef.None)
                continue;
            var elementLg = _elementScene.Instantiate<ElementLarge>();
            elementLg.Element = element;
            elementLg.Effectiveness = newDef.ModifiedValue;
            _elementDefContainer.Elements.AddChild(elementLg);
        }
        if (changed)
            _elementDefContainer.Dim = false;
    }
}
