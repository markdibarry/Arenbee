using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;

namespace Arenbee.Actors;

public class Actor : AActor
{
    public Actor(
        string actorId,
        string actorBodyId,
        string actorName,
        string equipmentSlotPresetId,
        AEquipment equipment,
        AInventory inventory,
        Godot.Collections.Array<Stat> stats,
        Godot.Collections.Array<Modifier> modifiers)
        : base(actorId, actorBodyId, actorName, equipmentSlotPresetId, equipment, inventory)
    {
        Stats = new Stats(this);
        InitStats();
        foreach (Stat stat in stats)
            Stats.StatLookup[stat.StatType] = new Stat(stat);
        foreach (Modifier mod in modifiers)
            Stats.AddMod(new Modifier(mod));
    }

    private void OnHPDepleted() => RaiseDefeated();

    public override void InitStats()
    {
        base.InitStats();
        ((Stats)Stats).HPDepleted += OnHPDepleted;
    }

    public override void SetActorBody(AActorBody? actorBody)
    {
        AItem? weapon = Equipment.GetSlot(EquipmentSlotCategoryIds.Weapon)?.Item;
        if (ActorBody is ActorBody oldActorBody)
        {
            if (weapon != null)
                oldActorBody.SetHoldItem(weapon, null);
            DamageRecieved -= oldActorBody.OnDamageReceived;
            //Defeated -= newActorBody.OnDefeated;
        }

        ActorBody = actorBody;

        if (ActorBody is ActorBody newActorBody)
        {
            if (weapon != null)
                newActorBody.SetHoldItem(null, weapon);
            DamageRecieved += newActorBody.OnDamageReceived;
            //Defeated += newActorBody.OnDefeated;
        }
    }

    protected override void OnEquipmentSet(EquipmentSlot slot, AItem? oldItem, AItem? newItem)
    {
        (ActorBody as ActorBody)?.HoldItemController.SetHoldItem(oldItem, newItem);
    }
}
