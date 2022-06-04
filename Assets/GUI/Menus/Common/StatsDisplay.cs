using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class StatsDisplay : PanelContainer
    {
        private PackedScene _elementScene;
        private GridContainer _gridContainer;
        private HBoxContainer _elementAtkContainer;
        private HBoxContainer _elementDefContainer;
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
            _gridContainer = GetNode<GridContainer>("VBoxContainer/GridContainer");
            _levelContainer = _gridContainer.GetNode<StatContainer>("Level");
            _hpContainer = _gridContainer.GetNode<PointContainer>("HP");
            _mpContainer = _gridContainer.GetNode<PointContainer>("MP");
            _attackContainer = _gridContainer.GetNode<StatContainer>("Attack");
            _defenseContainer = _gridContainer.GetNode<StatContainer>("Defense");
            _mAttackContainer = _gridContainer.GetNode<StatContainer>("M Attack");
            _mDefenseContainer = _gridContainer.GetNode<StatContainer>("M Defense");
            _elementAtkContainer = GetNode<HBoxContainer>("VBoxContainer/EAtk");
            _elementDefContainer = GetNode<HBoxContainer>("VBoxContainer/EDef");
        }

        public void UpdateStatsDisplay(Actor actor)
        {
            if (actor == null)
                return;
            UpdateStatsDisplay(actor.Stats, actor.Stats);
        }

        public void UpdateStatsDisplay(Stats stats, Stats mockStats)
        {
            if (stats == null)
                return;
            _levelContainer.UpdateDisplay(stats, mockStats, AttributeType.Level);
            _hpContainer.UpdateDisplay(stats, mockStats, AttributeType.HP);
            _mpContainer.UpdateDisplay(stats, mockStats, AttributeType.MP);
            _attackContainer.UpdateDisplay(stats, mockStats, AttributeType.Attack);
            _defenseContainer.UpdateDisplay(stats, mockStats, AttributeType.Defense);
            _mAttackContainer.UpdateDisplay(stats, mockStats, AttributeType.MagicAttack);
            _mDefenseContainer.UpdateDisplay(stats, mockStats, AttributeType.MagicDefense);
            AddEAtkContainer(mockStats);
            AddEDefContainer(mockStats);
        }

        private void AddEAtkContainer(Stats stats)
        {
            _elementAtkContainer.QueueFreeAllChildren();
            var atkLabel = new Label() { Text = "E.Atk:" };
            _elementAtkContainer.AddChild(atkLabel);
            var element = stats.ElementOffs.CurrentElement;
            if (element != ElementType.None)
            {
                var elementLg = _elementScene.Instantiate<ElementLarge>();
                elementLg.Element = element;
                _elementAtkContainer.AddChild(elementLg);
            }
        }

        private void AddEDefContainer(Stats stats)
        {
            _elementDefContainer.QueueFreeAllChildren();
            var defLabel = new Label() { Text = "E.Def:" };
            _elementDefContainer.AddChild(defLabel);
            foreach (var element in Enum<ElementType>.Values())
            {
                var elDef = stats.ElementDefs.GetStat(element);
                if (elDef == null)
                    continue;
                if (elDef.ModifiedValue == ElementDef.None) continue;
                var elementLg = _elementScene.Instantiate<ElementLarge>();
                elementLg.Element = element;
                elementLg.Effectiveness = elDef.ModifiedValue;
                _elementDefContainer.AddChild(elementLg);
            }
        }
    }
}
