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

        public void Update(Actor actor)
        {
            Update(actor, null, null);
        }

        public void Update(Actor actor, EquipmentSlot slot, string itemId)
        {
            var mockStats = actor.Stats;
            EquipmentSlot newSlot = null;
            if (slot != null)
            {
                mockStats = new Stats(actor.Stats);
                newSlot = new EquipmentSlot(slot);
                newSlot.Item?.ItemStats?.RemoveFromStats(mockStats);
                newSlot.SetItemById(itemId);
                newSlot.Item?.ItemStats?.AddToStats(mockStats);
                mockStats.UpdateStats();
            }
            _gridContainer.RemoveAllChildren();
            AddStatContainer(actor.Stats, mockStats, AttributeType.Level);
            _gridContainer.AddChild(new MarginContainer());
            AddPointContainer(actor.Stats, mockStats, AttributeType.HP);
            AddPointContainer(actor.Stats, mockStats, AttributeType.MP);
            AddStatContainer(actor.Stats, mockStats, AttributeType.Attack);
            AddStatContainer(actor.Stats, mockStats, AttributeType.Defense);
            AddStatContainer(actor.Stats, mockStats, AttributeType.MagicAttack);
            AddStatContainer(actor.Stats, mockStats, AttributeType.MagicDefense);
            AddEAtkContainer(actor, newSlot);
            AddEDefContainer(mockStats);
        }

        private void AddEAtkContainer(Actor actor, EquipmentSlot slot)
        {
            _elementAtkContainer.RemoveAllChildren();
            var atkLabel = new Label();
            atkLabel.Text = "E.Atk:";
            _elementAtkContainer.AddChild(atkLabel);
            Element? element;
            if (slot?.SlotType == ItemType.Weapon)
                element = slot.Item?.ItemStats?.ActionElement;
            else
                element = actor.GetAtkElement();
            if (element != null && element != Element.None)
            {
                var elementLg = _elementScene.Instantiate<ElementLarge>();
                elementLg.Element = (Element)element;
                _elementAtkContainer.AddChild(elementLg);
            }
        }

        private void AddEDefContainer(Stats stats)
        {
            _elementDefContainer.RemoveAllChildren();
            var defLabel = new Label();
            defLabel.Text = "E.Def:";
            _elementDefContainer.AddChild(defLabel);
            foreach (var element in Enum<Element>.Values())
            {
                int multiplier = stats.GetElementMultiplier(element) - 10;
                if (multiplier == ElementModifier.None) continue;
                var elementLg = _elementScene.Instantiate<ElementLarge>();
                elementLg.Element = element;
                elementLg.Effectiveness = multiplier;
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
            var currentValue = stats.GetAttribute(maxType).DisplayValue;
            var mockValue = mockStats.GetAttribute(maxType).DisplayValue;
            var pointContainer = _pointContainerScene.Instantiate<PointContainer>();
            _gridContainer.AddChild(pointContainer);
            pointContainer.StatNameLabel.Text = attributeType.Get().Abbreviation + ":";
            pointContainer.StatCurrentValueLabel.Text = stats.GetAttribute(attributeType).DisplayValue.ToString();
            DisplayValueColor(currentValue, mockValue, pointContainer.StatMaxValueLabel);
        }

        private void AddStatContainer(Stats stats, Stats mockStats, AttributeType attributeType)
        {
            var currentValue = stats.GetAttribute(attributeType).DisplayValue;
            var mockValue = mockStats.GetAttribute(attributeType).DisplayValue;
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
