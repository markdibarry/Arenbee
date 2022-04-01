using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public partial class HitBox : Area2D
    {
        [Export]
        public int InitialValue { get; private set; } = 1;
        public ActionData ActionData { get; set; }

        public override void _Ready()
        {
            base._Ready();
            ActionData = new ActionData(this, this, ActionType.Environment);
        }
    }
}
