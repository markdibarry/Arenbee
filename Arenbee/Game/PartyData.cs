using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Arenbee.Actors;
using Arenbee.Items;

namespace Arenbee;

public class PartyData
{
    public PartyData(Party party, int inventoryIndex)
    {
        PartyId = party.Id;
        ActorData = party.Actors.Select(x => new ActorData(x));
        InventoryIndex = inventoryIndex;
        Items = Array.Empty<ItemStackData>();
    }

    [JsonConstructor]
    public PartyData(string partyId, IEnumerable<ActorData> actorData, int inventoryIndex, IEnumerable<ItemStackData> items)
    {
        PartyId = partyId;
        ActorData = actorData;
        InventoryIndex = inventoryIndex;
        Items = items;
    }

    public string PartyId { get; set; }
    public IEnumerable<ActorData> ActorData { get; set; }
    public int InventoryIndex { get; set; } = -1;
    public IEnumerable<ItemStackData> Items { get; set; }

    public Party ToParty(IEnumerable<Inventory> inventories)
    {
        Inventory inventory = inventories.ElementAt(InventoryIndex);
        IEnumerable<Actor> actors = ActorData.Select(x => x.ToActor(inventory));
        return new(PartyId, actors, inventory);
    }
}
