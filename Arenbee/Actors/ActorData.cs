using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Actors;

public partial class ActorData : AActorData
{
    private static readonly AItemDB s_itemDB = Locator.ItemDB;
    private static readonly AActorDataDB s_actorDataDB = Locator.ActorDataDB;

    public ActorData() { }

    public ActorData(AActor actor)
    {
        ActorId = actor.ActorId;
        ActorName = actor.Name;
        StatsData = new StatsData((Stats)actor.Stats);
        List<EquipmentSlotData> equipmentSlotData = new();
        for (int i = 0; i < actor.Inventory.Items.Count; i++)
        {
            ItemStack itemStack = actor.Inventory.Items.ElementAt(i);
            foreach (Reservation reservation in itemStack.Reservations)
            {
                if (reservation.Actor == actor)
                    equipmentSlotData.Add(new(reservation.EquipmentSlot.SlotCategory.Id, i));
            }
        }

        EquipmentSlotData = equipmentSlotData;
    }

    [JsonConstructor]
    public ActorData(
        string actorId,
        string actorName,
        StatsData statsData,
        IEnumerable<EquipmentSlotData> equipmentSlotData,
        IEnumerable<ItemStackData> itemStackData)
    {
        ActorId = actorId;
        ActorName = actorName;
        StatsData = statsData;
        EquipmentSlotData = equipmentSlotData;
        ItemStackData = itemStackData;
    }

    public ActorData(ActorData actorData)
        : this(
              actorData.ActorId,
              actorData.ActorName,
              actorData.StatsData,
              actorData.EquipmentSlotData.Select(x => new EquipmentSlotData(x)),
              actorData.ItemStackData.Select(x => new ItemStackData(x)))
    {
    }

    [Export]
    public string ActorName { get; set; }
    [Export]
    public StatsData StatsData { get; set; }
    public IEnumerable<EquipmentSlotData> EquipmentSlotData { get; } = Array.Empty<EquipmentSlotData>();
    public IEnumerable<ItemStackData> ItemStackData { get; } = Array.Empty<ItemStackData>();

    public override AActorData Clone()
    {
        return new ActorData(this);
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
        actor.ActorBody = actorBody;
        actorBody.Actor = actor;
        return actor;
    }

    public AActor CreateActor(AInventory? externalInventory = null)
    {
        return CreateActor(s_itemDB, ((EquipmentSlotCategoryDB)Locator.EquipmentSlotCategoryDB).BasicEquipment, externalInventory);
    }

    public override AActor CreateActor(AItemDB itemDB, IEnumerable<EquipmentSlotCategory> equipmentSlotCategories, AInventory? externalInventory)
    {
        Inventory inventory = externalInventory as Inventory ?? new(ItemStackData.Select(x => x.CreateItemStack(itemDB)).OfType<ItemStack>());
        Equipment equipment = new(inventory, equipmentSlotCategories);
        Actor actor = new(
            actorId: ActorId,
            actorName: ActorName,
            equipment,
            inventory);

        foreach (Stat stat in StatsData.StatLookup)
            actor.Stats.StatLookup[stat.StatType] = new Stat(stat);
        foreach (Modifier mod in StatsData.Modifiers)
            actor.Stats.AddMod(new Modifier(mod));

        foreach (EquipmentSlotData slotData in EquipmentSlotData)
        {
            EquipmentSlot? slot = equipment.GetSlot(slotData.SlotCategoryId);
            if (slot == null)
                continue;
            slot.TrySetItem(actor, inventory.Items.ElementAt(slotData.ItemStackIndex));
        }

        return actor;
    }
}
