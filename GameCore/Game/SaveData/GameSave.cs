using System.Collections.Generic;
using GameCore.Actors;
using GameCore.Items;

namespace GameCore.SaveData;

public class GameSave
{
    public GameSave()
    {
        ActorData = new List<ActorData>();
    }

    public GameSave(GameSessionBase gameSession) : this()
    {
        Items = gameSession.Party?.Inventory?.Items;
        SessionState = gameSession.SessionState;
        foreach (ActorBase actor in gameSession.Party?.Actors)
            ActorData.Add(new ActorData(actor));
    }

    public SessionState SessionState { get; set; }
    public ICollection<ItemStack> Items { get; set; }
    public List<ActorData> ActorData { get; set; }
}
