using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class PointContainer : EqualContainer
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
        private string _statNameText;
        private string _statCurrentValueText;
        private string _statMaxValueText;
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
        public string StatCurrentValueText
        {
            get => _statCurrentValueText;
            set
            {
                _statCurrentValueText = value;
                if (StatCurrentValueLabel != null)
                    StatCurrentValueLabel.Text = _statCurrentValueText;
            }
        }
        [Export]
        public string StatMaxValueText
        {
            get => _statMaxValueText;
            set
            {
                _statMaxValueText = value;
                if (StatMaxValueLabel != null)
                    StatMaxValueLabel.Text = _statMaxValueText;
            }
        }
        public Label StatNameLabel { get; set; }
        public HBoxContainer ValueHBox { get; set; }
        public Label StatCurrentValueLabel { get; set; }
        public Label StatMaxValueLabel { get; set; }

        public override void _Ready()
        {
            base._Ready();
            ValueHBox = ValueContainer.GetNode<HBoxContainer>("HBoxContainer");
            StatNameLabel = GetNode<Label>("%Key");
            StatCurrentValueLabel = GetNode<Label>("%Current");
            StatMaxValueLabel = GetNode<Label>("%Max");
            StatNameLabel.Text = _statNameText;
            StatCurrentValueLabel.Text = _statCurrentValueText;
            StatMaxValueLabel.Text = _statMaxValueText;
            StatNameLabel.Resized += OnResize;
            ValueHBox.Resized += OnResize;
        }

        public override void OnResize()
        {
            ResizeItems(StatNameLabel, ValueHBox);
        }
    }
}
