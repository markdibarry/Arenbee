using Arenbee.Framework.Constants;
using Arenbee.Framework.Items;

namespace Arenbee.Assets.Items
{
    public partial class MetalHockeyStick : Weapon
    {
        public override void _Ready()
        {
            base._Ready();
            ItemId = "MetalHockeyStick";
            WeaponTypeName = WeaponTypeConstants.Spear;
            InitialState = new NotAttacking();
        }
    }
}
