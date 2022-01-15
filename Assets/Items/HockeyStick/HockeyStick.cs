using Arenbee.Assets.Items.HockeyStickNS;
using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;

namespace Arenbee.Assets.Items
{
    public partial class HockeyStick : Weapon
    {
        public override void _Ready()
        {
            base._Ready();
            WeaponTypeName = WeaponTypeConstants.Spear;
            InitialState = new NotAttacking();
        }

        public override void SetHitBoxAction()
        {
            HitBox.HitBoxAction = new HitBoxAction()
            {
                SourceInfo = new EventSourceInfo(Actor),
                ActionType = ActionType.Melee,
                Element = Element.Earth,
                Value = Actor.ActorStats.Stats[StatType.Attack].ModifiedValue
            };
        }
    }
}
