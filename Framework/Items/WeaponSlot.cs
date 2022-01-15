using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Framework.Items
{
    public partial class WeaponSlot : Node2D
    {
        private Actor _actor;
        public Weapon CurrentWeapon { get; set; }

        public override void _Ready()
        {
            base._Ready();
        }

        public void Init(Actor actor)
        {
            _actor = actor;
            if (GetChildCount() > 0)
            {
                Weapon weapon = GetChildOrNull<Weapon>(0);
                if (weapon != null) EquipWeapon(weapon);
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
            weapon.Actor = _actor;
            _actor.StateController.ActionStateMachine.Init(weapon.InitialState);
        }
    }
}
