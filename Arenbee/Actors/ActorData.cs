using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Items;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Actors;

public class ActorData : IActorData
{
    private static readonly AItemDB s_itemDB = Locator.ItemDB;
    private static readonly AActorDataDB s_actorDataDB = Locator.ActorDataDB;

    public ActorData(AActor actor)
    {
        ActorId = actor.ActorId;
        ActorName = actor.Name;
        Stats = actor.Stats;
        List<ItemStackData> itemStackData = new();
        List<EquipmentSlotData> equipmentSlotData = new();
        foreach (var itemStack in actor.Inventory.Items)
        {
            itemStackData.Add(new ItemStackData(itemStack));
            foreach (var reservation in itemStack.Reservations)
            {
                if (reservation.Actor == actor)
                    equipmentSlotData.Add(reservation.EquipmentSlot)
            }
        }
        ItemStackData = actor.Inventory.Items.Select(x => new ItemStackData(x));
        EquipmentSlotData = actor.Equipment.Slots.Select(x => new EquipmentSlotData(x, actor.Inventory.Items));
    }

    public ActorData(
        string actorId,
        string actorName,
        Stats stats,
        IEnumerable<EquipmentSlotData> equipmentSlotData,
        IEnumerable<ItemStackData> itemStackData)
    {
        ActorId = actorId;
        ActorName = actorName;
        Stats = stats;
        EquipmentSlotData = equipmentSlotData;
        ItemStackData = itemStackData;
    }

    public string ActorId { get; }
    public string ActorName { get; }
    public Stats Stats { get; }
    public IEnumerable<EquipmentSlotData> EquipmentSlotData { get; }
    public IEnumerable<ItemStackData> ItemStackData { get; }

    public IActorData Clone()
    {
        return new ActorData(
            ActorId,
            ActorName,
            new Stats(Stats),
            EquipmentSlotData.Select(x => x.Clone()),
            ItemStackData.Select(x => x.Clone()));
    }

    public static AActor? CreateActorAndBody(string actorId, PackedScene actorBodyScene)
    {
        if (s_actorDataDB.GetActorData(actorId) is not ActorData actorData)
        {
            GD.PrintErr($"{actorId} ActorData not found.");
            return null;
        }
        AActor actor = actorData.CreateActor();
        AActorBody actorBody = actorBodyScene.Instantiate<ActorBody>();
        actor.SetActorBody(actorBody);
        actorBody.Actor = actor;
        return actor;
    }

    public AActor CreateActor()
    {
        return CreateActor(s_itemDB, ((EquipmentSlotCategoryDB)Locator.EquipmentSlotCategoryDB).BasicEquipment);
    }

    public AActor CreateActor(AItemDB itemDB, IEnumerable<EquipmentSlotCategory> equipmentSlotCategories)
    {
        List<AItemStack> itemStacks = new();
        foreach (var itemStackData in ItemStackData)
        {
            AItemStack? itemStack = itemStackData.CreateItemStack(itemDB);
            if (itemStack != null)
                itemStacks.Add(itemStack);
        }

        List<EquipmentSlot> equipmentSlots = new();
        EquipmentSlotData[] equipmentSlotData = EquipmentSlotData.ToArray();
        foreach (EquipmentSlotCategory category in equipmentSlotCategories)
        {
            var slotData = Array.Find(equipmentSlotData, x => x.SlotCategoryId == category.Id);
            if (slotData == null || slotData.ItemStackIndex == -1)
                equipmentSlots.Add(new(category));
            else
                equipmentSlots.Add(new(category, itemStacks[slotData.ItemStackIndex]));
        }

        Inventory inventory = new(itemStacks);
        Equipment equipment = new(inventory, equipmentSlots.ToArray());
        Stats stats = new(Stats);
        Actor actor = new(
            actorId: ActorId,
            actorName: ActorName,
            equipment,
            inventory,
            stats);

        stats.StatsOwner = actor;

        foreach (var slot in equipmentSlots)
            slot.ItemStack?.AddReservation(actor, slot);

        return actor;
    }
}
