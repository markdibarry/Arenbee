using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Common
{
    [Tool]
    public partial class EqualContainer : MarginContainer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public MarginContainer KeyContainer { get; set; }
        public MarginContainer ValueContainer { get; set; }

        public override void _Ready()
        {
            base._Ready();
            KeyContainer = GetNode<MarginContainer>("HBoxContainer/KeyContainer");
            ValueContainer = GetNode<MarginContainer>("HBoxContainer/ValueContainer");
            Resized += OnResize;
        }

        public virtual void OnResize()
        {
            ResizeItems(KeyContainer, ValueContainer);
        }

        protected void ResizeItems(Control itemA, Control itemB)
        {
            if (itemA.RectSize.x > itemB.RectSize.x)
                RectMinSize = new Vector2(itemA.RectSize.x * 2, 0);
            else if (itemB.RectSize.x > itemA.RectSize.x)
                RectMinSize = new Vector2(itemB.RectSize.x * 2, 0);
        }
    }
}
