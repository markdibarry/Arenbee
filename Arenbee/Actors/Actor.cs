﻿using System;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Extensions;
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
        Stats = new Stats(this, stats, modifiers);
        InitStats();
    }

    public override ActorBody? ActorBody => ActorBodyInternal as ActorBody;
    public override Stats Stats { get; }

    public ActorBody CreateBody()
    {
        string? bodyPath = ActorBodyDB.ById(ActorBodyId);
        if (bodyPath == null)
            throw new Exception($"No Body {ActorBodyId} found.");
        ActorBody actorBody = GDEx.Instantiate<ActorBody>(bodyPath);
        SetActorBody(actorBody);
        actorBody.SetActor(this);
        return actorBody;
    }

    public override void InitStats()
    {
        base.InitStats();
        Stats.HPDepleted += OnHPDepleted;
    }

    public override void SetActorBody(AActorBody? actorBody)
    {
        AItem? weapon = Equipment.GetSlot(EquipmentSlotCategoryIds.Weapon)?.Item;
        if (ActorBody is ActorBody oldActorBody)
        {
            if (weapon != null)
                oldActorBody.SetHoldItem(weapon, null);
            DamageReceived -= oldActorBody.OnDamageReceived;
            //Defeated -= newActorBody.OnDefeated;
        }

        ActorBodyInternal = actorBody;

        if (ActorBody is ActorBody newActorBody)
        {
            if (weapon != null)
                newActorBody.SetHoldItem(null, weapon);
            DamageReceived += newActorBody.OnDamageReceived;
            //Defeated += newActorBody.OnDefeated;
        }
    }

    protected override void OnEquipmentSet(EquipmentSlot slot, AItem? oldItem, AItem? newItem)
    {
        ActorBody?.HoldItemController.SetHoldItem(oldItem, newItem);
    }

    private void OnHPDepleted() => RaiseDefeated();
}
