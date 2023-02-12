using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors;
using Arenbee.Game;
using Arenbee.Items;
using GameCore;
using GameCore.SaveData;

namespace Arenbee.SaveData;

public class GameSave : IGameSave
{
    public GameSave(
        int id,
        SessionState sessionState,
        IEnumerable<ItemStackData> items,
        IEnumerable<ActorData> actorData)
    {
        Id = id;
        SessionState = sessionState;
        Items = items;
        ActorData = actorData;
    }

    public GameSave(int id, GameSession gameSession)
    {
        PlayerParty party = gameSession.Party;
        Id = id;
        Items = party.Inventory.Items.Select(x => new ItemStackData(x));
        ActorData = party.Actors.Select(x => new ActorData(x));
        SessionState = gameSession.SessionState;
    }

    public int Id { get; }
    public SessionState SessionState { get; }
    public IEnumerable<ItemStackData> Items { get; }
    public IEnumerable<ActorData> ActorData { get; }
}
