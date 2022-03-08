using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class PointContainer : EqualContainer
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
        public Label StatNameLabel { get; set; }
        public HBoxContainer ValueHBox { get; set; }
        public Label StatCurrentValueLabel { get; set; }
        public Label StatMaxValueLabel { get; set; }

        public override void _Ready()
        {
            base._Ready();
            StatNameLabel = KeyContainer.GetNode<Label>("Key");
            ValueHBox = ValueContainer.GetNode<HBoxContainer>("HBoxContainer");
            StatCurrentValueLabel = ValueContainer.GetNode<Label>("HBoxContainer/Current");
            StatMaxValueLabel = ValueContainer.GetNode<Label>("HBoxContainer/Max");
            StatNameLabel.Resized += OnResize;
            ValueHBox.Resized += OnResize;
        }

        public override void OnResize()
        {
            ResizeItems(StatNameLabel, ValueHBox);
        }
    }
}
