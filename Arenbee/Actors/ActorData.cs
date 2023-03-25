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

    public ActorData() { }

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

    public ActorData(ActorData actorData)
        : this(
              actorData.ActorId,
              actorData.ActorName,
              actorData.StatsData,
              actorData.EquipmentSlotData.Select(x => new EquipmentSlotData(x)),
              actorData.ItemStackData.Select(x => new ItemStackData(x)))
    {
    }

    [Export] public string ActorName { get; set; } = string.Empty;
    [Export] public StatsData StatsData { get; set; } = null!;
    public IEnumerable<EquipmentSlotData> EquipmentSlotData { get; } = Array.Empty<EquipmentSlotData>();
    public IEnumerable<ItemStackData> ItemStackData { get; } = Array.Empty<ItemStackData>();

    public override AActorData Clone() => new ActorData(this);

    public AActor CreateActor(AInventory? externalInventory = null)
    {
        return CreateActor(EquipmentSlotPresetIds.BasicEquipment, externalInventory);
    }

    public override AActor CreateActor(string equipmentSlotPresetId, AInventory? externalInventory)
    {
        Inventory inventory = externalInventory as Inventory ?? new(ItemStackData.Select(x => x.CreateItemStack(s_itemDB)).OfType<ItemStack>());
        IReadOnlyCollection<EquipmentSlotCategory> equipmentPreset = Locator.EquipmentSlotCategoryDB.GetCategoryPreset(equipmentSlotPresetId);
        Equipment equipment = new(inventory, equipmentPreset);
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
