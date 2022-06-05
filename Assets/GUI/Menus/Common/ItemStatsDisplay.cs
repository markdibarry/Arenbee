using System.Linq;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Menus.Common;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class ItemStatsDisplay : PanelContainer
    {
        private PackedScene _elementScene;
        private GridContainer _gridContainer;
        private ElementContainer _elementAtkContainer;
        private ElementContainer _elementDefContainer;
        private StatContainer _hpContainer;
        private StatContainer _mpContainer;
        private StatContainer _attackContainer;
        private StatContainer _defenseContainer;
        private StatContainer _mAttackContainer;
        private StatContainer _mDefenseContainer;

        public override void _Ready()
        {
            base._Ready();
            _elementScene = GD.Load<PackedScene>(ElementLarge.GetScenePath());
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

        public void UpdateStatsDisplay(Item item)
        {
            UpdateAttributes(item);
            UpdateEAtk(item);
            UpdateEDef(item);
        }

        private void UpdateAttributes(Item item)
        {
            var attMods = item?.Modifiers?.Where(x => !x.IsHidden && x.StatType == StatType.Attribute);
            _hpContainer.UpdateDisplay(attMods, AttributeType.MaxHP);
            _mpContainer.UpdateDisplay(attMods, AttributeType.MaxMP);
            _attackContainer.UpdateDisplay(attMods, AttributeType.Attack);
            _defenseContainer.UpdateDisplay(attMods, AttributeType.Defense);
            _mAttackContainer.UpdateDisplay(attMods, AttributeType.MagicAttack);
            _mDefenseContainer.UpdateDisplay(attMods, AttributeType.MagicDefense);
        }

        private void UpdateEAtk(Item item)
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

        private void UpdateEDef(Item item)
        {
            _elementDefContainer.Elements.QueueFreeAllChildren();
            _elementDefContainer.Dim = true;
            var mods = item?.Modifiers?.Where(x => !x.IsHidden && x.StatType == StatType.ElementDef);
            if (mods == null || mods.Count() == 0)
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
}
