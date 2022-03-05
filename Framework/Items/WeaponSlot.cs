using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Framework.Items
{
    public partial class WeaponSlot : Node2D
    {
        private StateController _stateController;
        private Stats _stats;
        private Node2D _holder;
        public Weapon CurrentWeapon { get; private set; }

        public void Init(Node2D holder, Stats stats, StateController stateController)
        {
            _holder = holder;
            _stats = stats;
            _stateController = stateController;
        }

        public void SetWeapon(Item item)
        {
            if (item == null)
            {
                DetachWeapon();
                return;
            }
            SetWeapon(item.Id);
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
            weapon.Init(_holder, _stats);
            CurrentWeapon = weapon;
            AddChild(weapon);
            _stateController.ActionStateMachine.Init(weapon.InitialState);
        }

        private void DetachWeapon()
        {
            Weapon weapon = this.GetChildOrNullButActually<Weapon>(0);
            if (weapon != null)
            {
                RemoveChild(weapon);
                weapon.QueueFree();
                CurrentWeapon = null;
                _stateController.ActionStateMachine.Init(_stateController.UnarmedInitialState);
            }
        }
    }
}
