using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class Cursor : Node2D
    {
        public Sprite2D Sprite2D { get; set; }

        public override void _Ready()
        {
            Sprite2D = GetNodeOrNull<Sprite2D>("Sprite2D");
        }

        public override void _PhysicsProcess(float delta)
        {
            HandleCursorAnimation(delta);
        }

        public virtual void HandleCursorAnimation(float delta)
        {

        }
    }
}
