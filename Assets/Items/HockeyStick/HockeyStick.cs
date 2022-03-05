using Arenbee.Framework.Constants;
using Arenbee.Framework.Items;

namespace Arenbee.Assets.Items
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
    }
}
