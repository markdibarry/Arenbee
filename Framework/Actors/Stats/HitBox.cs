using Godot;

namespace Arenbee.Framework.Actors.Stats
{
    public partial class HitBox : Area2D
    {
        [Export]
        public int Value { get; set; }
        public HitBoxAction HitBoxAction { get; set; }

        public override void _Ready()
        {
            base._Ready();
            HitBoxAction = new HitBoxAction(this);
            HitBoxAction.Value = Value;
        }
    }
}
