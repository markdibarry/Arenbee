using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class StatContainer : EqualContainer
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
        private bool _dim;
        private string _statNameText;
        private string _statValueText;
        [Export]
        public string StatNameText
        {
            get => _statNameText;
            set
            {
                _statNameText = value;
                if (StatNameLabel != null)
                    StatNameLabel.Text = _statNameText;
            }
        }
        [Export]
        public string StatValueText
        {
            get => _statValueText;
            set
            {
                _statValueText = value;
                if (StatValueLabel != null)
                    StatValueLabel.Text = _statValueText;
            }
        }
        public Label StatNameLabel { get; set; }
        public Label StatValueLabel { get; set; }
        [Export]
        public bool Dim
        {
            get => _dim;
            set
            {
                _dim = value;
                Modulate = _dim ? ColorConstants.DimGrey : Colors.White;
            }
        }

        public override void _Ready()
        {
            base._Ready();
            StatNameLabel = GetNode<Label>("%Key");
            StatValueLabel = GetNode<Label>("%Value");
            StatNameLabel.Text = _statNameText;
            StatValueLabel.Text = _statValueText;
            StatNameLabel.Resized += OnResize;
            StatValueLabel.Resized += OnResize;
        }

        public override void OnResize()
        {
            ResizeItems(StatNameLabel, StatValueLabel);
        }

        public void UpdateDisplay(IEnumerable<Modifier> mods, AttributeType attributeType)
        {
            Dim = true;
            StatNameText = attributeType.Get().Abbreviation + ":";
            StatValueText = "0";
            var mod = mods?.FirstOrDefault(x => (AttributeType)x.SubType == attributeType);
            var value = mod != null ? mod.Value : 0;
            StatValueText = value.ToString();
            if (value != 0)
                Dim = false;
            DisplayValueColor(0, value);
        }

        public void UpdateDisplay(Stats stats, Stats mockStats, AttributeType attributeType)
        {
            var currentValue = stats.Attributes.GetStat(attributeType).DisplayValue;
            var mockValue = mockStats.Attributes.GetStat(attributeType).DisplayValue;
            StatNameText = attributeType.Get().Abbreviation + ":";
            StatValueText = mockValue.ToString();
            DisplayValueColor(currentValue, mockValue);
        }

        private void DisplayValueColor(int currentValue, int mockValue)
        {
            if (mockValue > currentValue)
                StatValueLabel.Modulate = ColorConstants.TextGreen;
            else if (mockValue < currentValue)
                StatValueLabel.Modulate = ColorConstants.TextRed;
            else
                StatValueLabel.Modulate = Colors.White;
        }
    }
}
