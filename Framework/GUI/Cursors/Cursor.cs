using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class Cursor : Node2D
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public Sprite2D Sprite2D { get; set; }
        public bool FlashEnabled { get; set; }

        public override void _Ready()
        {
            Sprite2D = GetNodeOrNull<Sprite2D>("Sprite2D");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (FlashEnabled)
                Visible = !Visible;
            HandleCursorAnimation(delta);
        }

        public virtual void HandleCursorAnimation(float delta) { }
    }
}
