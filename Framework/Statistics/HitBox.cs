using Godot;

namespace Arenbee.Framework.Statistics
{
    public partial class HitBox : Area2D
    {
        [Export]
        public int InitialValue { get; private set; } = 1;
        public ActionInfo ActionInfo { get; set; }

        public override void _Ready()
        {
            base._Ready();
            ActionInfo = new ActionInfo(this, this);
        }
    }
}
