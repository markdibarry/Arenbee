using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class StatContainer : EqualContainer
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
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

        public override void _Ready()
        {
            base._Ready();
            StatNameLabel = GetNode<Label>("%Key");
            StatValueLabel = GetNode<Label>("%Value");
            _statNameText = StatNameLabel.Text;
            _statValueText = StatValueLabel.Text;
            StatNameLabel.Resized += OnResize;
            StatValueLabel.Resized += OnResize;
        }

        public override void OnResize()
        {
            ResizeItems(StatNameLabel, StatValueLabel);
        }
    }
}
