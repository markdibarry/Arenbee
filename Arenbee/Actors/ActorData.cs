using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;
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
        string actorBodyId,
        string equipmentSlotPresetId,
        StatsData statsData,
        IEnumerable<EquipmentSlotData> equipmentSlotData,
        IEnumerable<ItemStackData> itemStackData)
    {
        ActorId = actorId;
        ActorName = actorName;
        ActorBodyId = actorBodyId;
        StatsData = statsData;
        EquipmentSlotData = equipmentSlotData;
        ItemStackData = itemStackData;
        EquipmentSlotPresetId = equipmentSlotPresetId;
    }

    public ActorData(Actor actor)
    {
        ActorId = actor.ActorId;
        ActorName = actor.Name;
        ActorBodyId = actor.ActorBodyId;
        EquipmentSlotPresetId = actor.EquipmentSlotPresetId;
        StatsData = new StatsData(actor.Stats);
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
              actorData.ActorBodyId,
              actorData.ActorName,
              actorData.EquipmentSlotPresetId,
              actorData.StatsData,
              actorData.EquipmentSlotData.Select(x => new EquipmentSlotData(x)),
              actorData.ItemStackData.Select(x => new ItemStackData(x)))
    {
    }

    [Export] public string ActorName { get; set; } = string.Empty;
    [Export] public StatsData StatsData { get; set; } = null!;
    public IEnumerable<EquipmentSlotData> EquipmentSlotData { get; } = Array.Empty<EquipmentSlotData>();
    public IEnumerable<ItemStackData> ItemStackData { get; } = Array.Empty<ItemStackData>();

    public override ActorData Clone() => new(this);

    public override Actor CreateActor(AInventory? externalInventory = null)
    {
        Inventory inventory = externalInventory as Inventory ?? new(ItemStackData.Select(x => x.CreateItemStack(s_itemDB)).OfType<ItemStack>());
        IReadOnlyCollection<EquipmentSlotCategory> equipmentPreset = Locator.EquipmentSlotCategoryDB.GetCategoryPreset(EquipmentSlotPresetId);
        Equipment equipment = new(inventory, equipmentPreset);
        Actor actor = new(
            actorId: ActorId,
            actorBodyId: ActorBodyId,
            actorName: ActorName,
            equipmentSlotPresetId: EquipmentSlotPresetId,
            equipment: equipment,
            inventory: inventory,
            stats: StatsData.StatLookup,
            modifiers: StatsData.Modifiers);

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
