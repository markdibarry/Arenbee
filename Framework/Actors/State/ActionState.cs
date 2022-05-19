using Arenbee.Framework.Items;

namespace Arenbee.Framework.Actors
{
    public abstract class ActionState : ActorState<ActionState, ActionStateMachineBase>
    {
        public Weapon Weapon => Actor.WeaponSlot.CurrentWeapon;

        protected override void PlayAnimation(string animationName)
        {
            StateController.PlayActionAnimation(animationName);
        }
    }
}