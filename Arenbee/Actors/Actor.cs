﻿using System.Collections.Generic;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;

namespace Arenbee.Actors;

public class Actor : BaseActor
{
    public Actor(
        string actorId,
        string actorBodyId,
        string actorName,
        string equipmentSlotPresetId,
        Equipment equipment,
        Inventory inventory,
        IEnumerable<Stat> stats,
        IEnumerable<Modifier> modifiers)
        : base(actorId, actorBodyId, actorName, equipmentSlotPresetId, inventory)
    {
        Equipment = equipment;
        Stats = new Stats(this, stats, modifiers);
        Init();
    }

    public override ActorBody? ActorBody => base.ActorBody as ActorBody;
    public override Equipment Equipment { get; }
    public override Stats Stats { get; }

    public override void InitStats()
    {
        base.InitStats();
        Stats.HPDepleted += OnHPDepleted;
    }

    public override void SetRole(int role, bool setActorBodyRole = true)
    {
        Role = role;
        if (setActorBodyRole)
            ActorBody?.SetRole(role, false);
    }

    public override void SetActorBody(BaseActorBody? actorBody)
    {
        BaseItem? weapon = Equipment.GetSlot(EquipmentSlotCategoryIds.Weapon)?.Item;
        if (ActorBody is ActorBody oldActorBody)
        {
            if (weapon != null)
                oldActorBody.SetHoldItem(weapon, null);
            DamageReceived -= oldActorBody.OnDamageReceived;
        }

        base.SetActorBody(actorBody);

        if (ActorBody is ActorBody newActorBody)
        {
            if (weapon != null)
                newActorBody.SetHoldItem(null, weapon);
            DamageReceived += newActorBody.OnDamageReceived;
        }
    }

    protected override void OnEquipmentSet(EquipmentSlot slot, BaseItem? oldItem, BaseItem? newItem)
    {
        ActorBody?.HoldItemController.SetHoldItem(oldItem, newItem);
    }

    private void OnHPDepleted() => RaiseDefeated();
}
