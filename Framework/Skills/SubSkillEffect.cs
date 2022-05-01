using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Statistics;

namespace Arenbee.Framework.Skills
{
    public static class SubSkillEffect
    {
        public static void RestoreHP(Actor user, Actor target, int value1, int value2)
        {
            target.Stats.ReceiveAction(new ActionData()
            {
                SourceName = target.Name,
                ActionType = ActionType.Item,
                Value = value1,
                ElementDamage = ElementType.Healing
            });
        }

        public static void RestoreHPPercent(Actor user, Actor target, int value1, int value2)
        {
            target.Stats.ReceiveAction(new ActionData()
            {
                SourceName = target.Name,
                ActionType = ActionType.Item,
                Value = value1,
                ElementDamage = ElementType.Healing
            });
        }
    }
}