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
        }

        public void SetWeapon(Item item)
        {
            if (item != null)
                AttachWeapon(item.Id);
            else
                DetachWeapon();
        }

        private void AttachWeapon(string itemId)
        {
            DetachWeapon();
            if (string.IsNullOrEmpty(itemId))
                return;
            var weapon = GDEx.Instantiate<Weapon>($"{PathConstants.ItemPath}{itemId}/{itemId}.tscn");
            if (weapon == null)
            {
                GD.PrintErr("No weapon at provided location!");
                return;
            }
            weapon.Init(_holder);
            CurrentWeapon = weapon;
            AddChild(weapon);
            _holder.StateController.SwitchActionStateMachine(weapon.GetActionStateMachine());
        }

        private void DetachWeapon()
        {
            Weapon weapon = this.GetChildOrNullButActually<Weapon>(0);
            if (weapon != null)
            {
                _holder.StateController.SwitchActionStateMachine(_holder.GetActionStateMachine());
                RemoveChild(weapon);
                weapon.QueueFree();
                CurrentWeapon = null;
            }
        }
    }
}
