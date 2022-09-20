using System.Collections.Generic;
using GameCore.Actors;
using GameCore.Items;

namespace GameCore.SaveData;

public class GameSave
{
    public GameSave()
    {
        ActorData = new();
    }

    public GameSave(List<ActorBase> actors, ICollection<ItemStack> items)
    {
        ActorData = new();
        SessionState = new SessionState();
        foreach (ActorBase actor in actors)
            ActorData.Add(new ActorData(actor));
        Items = items;
    }

    public GameSave(int id, GameSessionBase gameSession) : this()
    {
        Id = id;
        Items = gameSession.Party?.Inventory?.Items;
        SessionState = gameSession.SessionState;
        foreach (ActorBase actor in gameSession.Party?.Actors)
            ActorData.Add(new ActorData(actor));
    }

    public int Id { get; set; }
    public SessionState SessionState { get; set; }
    public ICollection<ItemStack> Items { get; set; }
    public List<ActorData> ActorData { get; set; }
}
