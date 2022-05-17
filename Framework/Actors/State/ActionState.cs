using Arenbee.Framework.Items;

namespace Arenbee.Framework.Actors
{
    public abstract class ActionState : ActorState
    {
        protected Weapon Weapon { get; private set; }

        public override void Init()
        {
            base.Init();
            Weapon = Actor.WeaponSlot.CurrentWeapon;
        }

        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayActionAnimation(animationName);
        }
    }
}