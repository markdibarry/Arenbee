using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.Items
{
    public partial class WeaponSlot : Node2D
    {
        private Actor _holder;
        public Weapon CurrentWeapon { get; private set; }

        public void Init(Actor holder)
        {
            _holder = holder;
            var slot = _holder.Equipment.GetSlot(EquipSlotName.Weapon);
            SetWeapon(slot.Item);
            _holder.Stats.RecalculateStats(true);
        }

        public void SetWeapon(Item item)
        {
            if (item != null)
                SetWeapon(item.Id);
            else
                DetachWeapon();
        }

        public void SetWeapon(string itemId)
        {
            DetachWeapon();
            if (string.IsNullOrEmpty(itemId))
                return;
            AttachWeapon(itemId);
        }

        private void AttachWeapon(string itemId)
        {
            var weapon = GDEx.Instantiate<Weapon>($"{PathConstants.ItemPath}{itemId}/{itemId}.tscn");
            if (weapon == null)
            {
                GD.PrintErr("No weapon at provided location!");
                return;
            }
            weapon.Init(_holder);
            CurrentWeapon = weapon;
            AddChild(weapon);
            _holder.StateController.ActionStateMachine.Init(weapon.InitialState);
        }

        private void DetachWeapon()
        {
            Weapon weapon = this.GetChildOrNullButActually<Weapon>(0);
            if (weapon != null)
            {
                RemoveChild(weapon);
                weapon.QueueFree();
                CurrentWeapon = null;
                _holder.StateController.ActionStateMachine.Init(_holder.StateController.UnarmedInitialState);
            }
        }
    }
}
