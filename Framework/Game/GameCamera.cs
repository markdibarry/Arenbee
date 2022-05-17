using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameCamera : Camera2D
    {
        public Node2D CurrentTarget { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            if (IsInstanceValid(CurrentTarget))
                GlobalPosition = CurrentTarget.GlobalPosition;
        }
    }
}