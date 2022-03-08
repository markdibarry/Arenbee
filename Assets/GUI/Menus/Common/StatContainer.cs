using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class StatContainer : EqualContainer
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
        public Label StatNameLabel { get; set; }
        public Label StatValueLabel { get; set; }

        public override void _Ready()
        {
            base._Ready();
            StatNameLabel = KeyContainer.GetNode<Label>("Key");
            StatValueLabel = ValueContainer.GetNode<Label>("Value");
            StatNameLabel.Resized += OnResize;
            StatValueLabel.Resized += OnResize;
        }

        public override void OnResize()
        {
            ResizeItems(StatNameLabel, StatValueLabel);
        }
    }
}
