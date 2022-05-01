using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public partial class HitBox : Area2D
    {
        public HitBox()
        {
            ActionData = new ActionData()
            {
                ActionType = ActionType.Environment,
                ElementDamage = ElementType.None,
                SourceName = Name,
                StatusEffectDamage = StatusEffectType.None,
                Value = InitialValue
            };
        }
        [Export]
        public int InitialValue { get; private set; } = 1;
        public ActionData ActionData { get; set; }

        public ActionData GetActionData()
        {
            ActionData.SourcePosition = GlobalPosition;
            return ActionData;
        }
    }
}
