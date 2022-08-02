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
            if (itemA.Size.x > itemB.Size.x)
                CustomMinimumSize = new Vector2(itemA.Size.x * 2, 0);
            else if (itemB.Size.x > itemA.Size.x)
                CustomMinimumSize = new Vector2(itemB.Size.x * 2, 0);
        }
    }
}
