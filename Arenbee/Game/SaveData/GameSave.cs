using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Arenbee.Game;
using Arenbee.Items;
using GameCore.Items;
using GameCore.SaveData;

namespace Arenbee.SaveData;

public class GameSave : IGameSave
{
    public GameSave(int id, GameSession gameSession)
    {
        Id = id;
        LastModifiedUtc = DateTime.UtcNow;
        List<Inventory> inventories = gameSession.Parties.Select(x => x.Inventory)
            .Distinct()
            .ToList();
        MainPartyId = gameSession.MainParty?.Id ?? string.Empty;
        Parties = gameSession.Parties
            .Select(x => new PartyData(x, inventories.IndexOf(x.Inventory)));
        Inventories = inventories.Select(x => new InventoryData(x));
        SessionState = gameSession.SessionState;
    }

    [JsonConstructor]
    public GameSave(
        int id,
        DateTime lastModifiedUtc,
        SessionState sessionState,
        string mainPartyId,
        IEnumerable<PartyData> parties,
        IEnumerable<InventoryData> inventories)
    {
        Id = id;
        LastModifiedUtc = lastModifiedUtc;
        SessionState = sessionState;
        MainPartyId = mainPartyId;
        Parties = parties;
        Inventories = inventories;
    }

    public int Id { get; }
    public DateTime LastModifiedUtc { get; }
    public SessionState SessionState { get; }
    public string MainPartyId { get; }
    public IEnumerable<PartyData> Parties { get; }
    public IEnumerable<InventoryData> Inventories { get; }
}
