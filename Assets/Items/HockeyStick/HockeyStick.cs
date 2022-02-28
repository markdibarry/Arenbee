using Arenbee.Framework.Statistics;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public partial class HockeyStick : Weapon
    {
        public override void _Ready()
        {
            base._Ready();
            ItemId = "HockeyStick";
            WeaponTypeName = WeaponTypeConstants.Spear;
            InitialState = new NotAttacking();
        }

        public override void UpdateHitBoxAction()
        {
            HitBox.HitBoxAction = new HitBoxAction(HitBox, Holder)
            {
                ActionType = ActionType.Melee,
                Element = Element.Earth,
                Value = Stats.Attributes[AttributeType.Attack].ModifiedValue
            };
        }
    }
}
