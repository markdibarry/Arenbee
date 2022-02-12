using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.Items
{
    public partial class WeaponSlot : Node2D
    {
        private Actor _actor;
        public Weapon CurrentWeapon { get; private set; }

        public void Init(Actor actor, EquipmentSlot equippedWeapon)
        {
            _actor = actor;
            if (!string.IsNullOrEmpty(equippedWeapon.ItemId))
            {
                AttachWeapon(equippedWeapon.ItemId);
            }
        }

        public void AttachWeapon(Item item)
        {
            AttachWeapon(item.Id);
        }

        public void AttachWeapon(string itemId)
        {
            DetachWeapon();
            var weapon = GDEx.Instantiate<Weapon>($"{PathConstants.ItemPath}{itemId}/{itemId}.tscn");
            CurrentWeapon = weapon;
            weapon.Actor = _actor;
            AddChild(weapon);
            _actor.StateController.ActionStateMachine.Init(weapon.InitialState);
        }

        public void DetachWeapon()
        {
            Weapon weapon = this.GetChildOrNullButActually<Weapon>(0);
            if (weapon != null)
            {
                RemoveChild(weapon);
                weapon.QueueFree();
                CurrentWeapon = null;
                _actor.StateController.ActionStateMachine.Init(_actor.StateController.UnarmedInitialState);
            }
        }
    }
}
