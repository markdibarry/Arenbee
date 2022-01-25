using Godot;

namespace Arenbee.Framework.Actors.Stats
{
    public partial class HitBox : Area2D
    {
        [Export]
        public int InitialValue { get; private set; } = 1;
        public HitBoxAction HitBoxAction { get; set; }

        public override void _Ready()
        {
            base._Ready();
            HitBoxAction = new HitBoxAction(this, this);
        }
    }
}
