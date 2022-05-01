using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class StatsDisplay : PanelContainer
    {
        private PackedScene _elementScene;
        private PackedScene _pointContainerScene;
        private PackedScene _statContainerScene;
        private GridContainer _gridContainer;
        private HBoxContainer _elementAtkContainer;
        private HBoxContainer _elementDefContainer;

        public override void _Ready()
        {
            base._Ready();
            _elementScene = GD.Load<PackedScene>(ElementLarge.GetScenePath());
            _pointContainerScene = GD.Load<PackedScene>(PointContainer.GetScenePath());
            _statContainerScene = GD.Load<PackedScene>(StatContainer.GetScenePath());
            _gridContainer = GetNode<GridContainer>("VBoxContainer/GridContainer");
            _elementAtkContainer = GetNode<HBoxContainer>("VBoxContainer/EAtk");
            _elementDefContainer = GetNode<HBoxContainer>("VBoxContainer/EDef");
        }

        public void UpdateStatsDisplay(Actor actor)
        {
            if (actor == null) return;
            UpdateStatsDisplay(actor.Stats, actor.Stats);
        }

        public void UpdateStatsDisplay(Stats stats, Stats mockStats)
        {
            if (stats == null) return;
            _gridContainer.QueueFreeAllChildren();
            AddStatContainer(stats, mockStats, AttributeType.Level);
            _gridContainer.AddChild(new MarginContainer());
            AddPointContainer(stats, mockStats, AttributeType.HP);
            AddPointContainer(stats, mockStats, AttributeType.MP);
            AddStatContainer(stats, mockStats, AttributeType.Attack);
            AddStatContainer(stats, mockStats, AttributeType.Defense);
            AddStatContainer(stats, mockStats, AttributeType.MagicAttack);
            AddStatContainer(stats, mockStats, AttributeType.MagicDefense);
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

        private void AddPointContainer(Stats stats, Stats mockStats, AttributeType attributeType)
        {
            AttributeType maxType;
            if (attributeType == AttributeType.HP)
                maxType = AttributeType.MaxHP;
            else if (attributeType == AttributeType.MP)
                maxType = AttributeType.MaxMP;
            else
                return;
            var currentValue = stats.Attributes.GetStat(maxType).DisplayValue;
            var mockValue = mockStats.Attributes.GetStat(maxType).DisplayValue;
            var pointContainer = _pointContainerScene.Instantiate<PointContainer>();
            _gridContainer.AddChild(pointContainer);
            pointContainer.StatNameLabel.Text = attributeType.Get().Abbreviation + ":";
            pointContainer.StatCurrentValueLabel.Text = stats.Attributes.GetStat(attributeType).DisplayValue.ToString();
            DisplayValueColor(currentValue, mockValue, pointContainer.StatMaxValueLabel);
        }

        private void AddStatContainer(Stats stats, Stats mockStats, AttributeType attributeType)
        {
            var currentValue = stats.Attributes.GetStat(attributeType).DisplayValue;
            var mockValue = mockStats.Attributes.GetStat(attributeType).DisplayValue;
            var statContainer = _statContainerScene.Instantiate<StatContainer>();
            _gridContainer.AddChild(statContainer);
            statContainer.StatNameLabel.Text = attributeType.Get().Abbreviation + ":";
            DisplayValueColor(currentValue, mockValue, statContainer.StatValueLabel);
        }

        private void DisplayValueColor(int currentValue, int mockValue, Label label)
        {
            label.Text = mockValue.ToString();
            if (mockValue > currentValue)
                label.Modulate = ColorConstants.TextGreen;
            else if (mockValue < currentValue)
                label.Modulate = ColorConstants.TextRed;
        }
    }
}
