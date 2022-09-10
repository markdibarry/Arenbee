using Arenbee.Items;
using GameCore.Actors;
using GameCore.Extensions;
using Godot;

namespace GameCore.Items;

public partial class WeaponSlot : Node2D
{
    private Actor _holder;
    public Weapon CurrentWeapon { get; private set; }

    public void Init(Actor holder)
    {
        _holder = holder;
        var slot = _holder.Equipment.GetSlot(EquipmentSlotCategoryIds.Weapon);
        SetWeapon(slot.Item);
    }

    public void SetWeapon(ItemBase item)
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
        var weapon = GDEx.Instantiate<Weapon>($"{Config.ItemPath}{itemId}/{itemId}.tscn");
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
        Weapon weapon = GetChildOrNull<Weapon>(0);
        if (weapon != null)
        {
            _holder.StateController.ResetActionStateMachine();
            RemoveChild(weapon);
            weapon.QueueFree();
            CurrentWeapon = null;
        }
    }
}
