using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public partial class HitBox : AreaBox
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
            GetActionData = () =>
            {
                var actionData = ActionData;
                actionData.SourcePosition = GlobalPosition;
                return actionData;
            };
        }

        [Export]
        public int InitialValue { get; private set; } = 1;
        public ActionData ActionData { get; set; }
        public Func<ActionData> GetActionData { get; set; }

        public void SetBasicMeleeBox(Actor actor)
        {
            var actionData = ActionData;
            actionData.SourceName = actor.Name;
            actionData.ActionType = ActionType.Melee;
            GetActionData = () =>
            {
                actionData.SourcePosition = GlobalPosition;
                actionData.ElementDamage = actor.Stats.ElementOffs.CurrentElement;
                actionData.StatusEffects = actor.Stats.StatusEffectOffs.GetModifiers();
                actionData.Value = actor.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue;
                return actionData;
            };
        }
    }
}
